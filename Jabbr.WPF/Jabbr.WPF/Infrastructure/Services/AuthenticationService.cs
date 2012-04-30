using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JabbR.Client;
using JabbR.Client.Models;
using Jabbr.WPF.Users;
using Newtonsoft.Json.Linq;

namespace Jabbr.WPF.Infrastructure.Services
{
    public class AuthenticationService : BaseService
    {
        private readonly JabbRClient _client;
        private readonly RoomService _roomService;
        private readonly UserService _userService;

        private LogOnInfo _logOnInfo;

        public AuthenticationService(
            JabbRClient jabbrClient,
            UserService userService,
            RoomService roomService)
        {
            _client = jabbrClient;
            _userService = userService;
            _roomService = roomService;
        }

        public UserViewModel CurrentUser { get; private set; }
        public event EventHandler<LoginCompleteEventArgs> SignInComplete;

        public Task<UserViewModel> SignIn(string token)
        {
            var taskCompletionSource = new TaskCompletionSource<UserViewModel>();

            Task<string> tokenAuthentication = Task.Factory.StartNew(() => AuthenticateToken(token));
            tokenAuthentication.ContinueWith(
                failedTokenTask => HandleSigninException(failedTokenTask.Exception, taskCompletionSource),
                TaskContinuationOptions.OnlyOnFaulted);
            tokenAuthentication.ContinueWith(tokenTask =>
            {
                Task<LogOnInfo> signinTask = _client.Connect(tokenTask.Result);
                SetupSigninTaskContinuations(signinTask, taskCompletionSource);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            return taskCompletionSource.Task;
        }

        public Task<UserViewModel> SignIn(string username, string password)
        {
            var signinTaskSource = new TaskCompletionSource<UserViewModel>();

            SetupSigninTaskContinuations(_client.Connect(username, password), signinTaskSource);

            return signinTaskSource.Task;
        }

        public void SignOut()
        {
            _client.Disconnect();
        }

        private void SetupSigninTaskContinuations(Task<LogOnInfo> signinTask,
                                                  TaskCompletionSource<UserViewModel> taskCompletionSource)
        {
            signinTask.ContinueWith(completedTask => CompleteSignin(completedTask.Result, taskCompletionSource),
                                    TaskContinuationOptions.OnlyOnRanToCompletion);
            signinTask.ContinueWith(failedTask => HandleSigninException(failedTask.Exception, taskCompletionSource),
                                    TaskContinuationOptions.OnlyOnFaulted);

            taskCompletionSource.Task.ContinueWith(
                completedSignin => OnSigninComplete(completedSignin.Result, _logOnInfo.Rooms.Any()),
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void CompleteSignin(LogOnInfo logOnInfo, TaskCompletionSource<UserViewModel> taskCompletionSource)
        {
            _logOnInfo = logOnInfo;

            User userinfo = _client.GetUserInfo().Result;
            UserViewModel userviewModel = _userService.GetUserViewModel(userinfo);
            userviewModel.IsCurrentUser = true;

            _roomService.JoinRooms(logOnInfo.Rooms);
            _roomService.GetRooms();

            CurrentUser = userviewModel;

            taskCompletionSource.TrySetResult(userviewModel);
        }

        private void HandleSigninException(Exception exception, TaskCompletionSource<UserViewModel> taskCompletionSource)
        {
            taskCompletionSource.TrySetException(exception);
        }

        private string AuthenticateToken(string token)
        {
            var cookieContainer = new CookieContainer();

            var request = (HttpWebRequest) WebRequest.Create(_client.SourceUrl + "/Auth/Login.ashx");
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            string data = "token=" + token;
            byte[] postBytes = Encoding.Default.GetBytes(data);

            request.ContentLength = postBytes.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postBytes, 0, postBytes.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            response.Close();

            CookieCollection cookies = cookieContainer.GetCookies(new Uri(_client.SourceUrl));
            string cookieValue = cookies[0].Value;

            JObject jsonObject = JObject.Parse(cookieValue);

            return (string) jsonObject["userId"];
        }

        private void OnSigninComplete(UserViewModel user, bool hasJoinedRooms)
        {
            EventHandler<LoginCompleteEventArgs> handler = SignInComplete;
            if (handler != null)
                PostOnUi(() => handler(this, new LoginCompleteEventArgs(user, hasJoinedRooms)));
        }
    }
}