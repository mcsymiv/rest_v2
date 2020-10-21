using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Serega.Steps
{
    [Binding]
    public class ApiTestStepsSteps
    {
        RestClient client;

        IRestResponse response;
        string randomString;
        Regex rgx;

        Dictionary<string, string> UsersData = new Dictionary<string, string>();
        Dictionary<string, object> CompanyData = new Dictionary<string, object>();
        Dictionary<string, object> UserData = new Dictionary<string, object>();
        Dictionary<string, object> TaskData = new Dictionary<string, object>();
        Dictionary<string, object> UserWithTaskData = new Dictionary<string, object>();

        [Given(@"A new client is created")]
        public void GivenANewClientIsCreated()
        {
            client = new RestClient(" http://users.bugred.ru/");
            randomString = DateTime.Now.ToString();
            rgx = new Regex("[^a-zA-Z0-9]+");
            randomString = rgx.Replace(randomString, "");
        }

        [Given(@"A user data is generated")]
        public void GivenAUserDataIsGenerated()
        {
            UsersData.Add("name", $"user{randomString}");
            UsersData.Add("email", $"{randomString}@mail.com");
            UsersData.Add("password", $"{randomString}qwerty");
        }

        [When(@"I send user credentials to the site")]
        public void WhenISendUserCredentialsToTheSite()
        {
            RestRequest request = new RestRequest("/tasks/rest/doregister", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(UsersData);
            response = client.Execute(request);
        }

        [Then(@"I get response with user information")]
        public void ThenIGetResponseWithUserInformation()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual($"user{randomString}", json["name"].ToString());
            Assert.AreEqual($"{randomString}@mail.com", json["email"].ToString());
        }

        [Given(@"A company data is generated")]
        public void GivenACompanyDataIsGenerated()
        {
            CompanyData.Add("company_name", "Company test name");
            CompanyData.Add("company_type", "ООО");
            CompanyData.Add("company_users", new List<string>() { "sdsad@gmail.ru" });
            CompanyData.Add("email_owner", "maksymiv@test.com");
        }

        [When(@"I sent POST request with company data")]
        public void WhenISentPOSTRequestWithCompanyData()
        {
            RestRequest request = new RestRequest("/tasks/rest/createcompany", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(CompanyData);
            response = client.Execute(request);
        }

        [Then(@"I get response contain (.*) name")]
        public void ThenIGetResponseContainName(string companyName)
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual("success", json["type"].ToString());
            Assert.AreEqual(companyName, json["company"]["name"].ToString());
        }

        [Given(@"Generated user infomation")]
        public void GivenGeneratedUserInfomation()
        {
            UserData.Add("email", $"user{randomString}@mail.{randomString.Substring(randomString.Length - 2, 2)}");
            UserData.Add("name", $"user{randomString}");
            UserData.Add("tasks", new List<int> { 23 });
            UserData.Add("companies", new List<int> { 55 });
        }

        [When(@"I send POST request with user data")]
        public void WhenISendPOSTRequestWithUserData()
        {
            RestRequest request = new RestRequest("tasks/rest/createuser", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(UserData);
            response = client.Execute(request);
        }

        [Then(@"I get back user information in the response body")]
        public void ThenIGetBackUserInformationInTheResponseBody()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            string userEmail = $"user{randomString}@mail.{randomString.Substring(randomString.Length - 2, 2)}";
            Assert.AreEqual(userEmail.ToLower(), json["email"].ToString());
            Assert.AreEqual($"user{randomString}", json["name"].ToString());
        }

        [Given(@"A task data is generated")]
        public void GivenATaskDataIsGenerated()
        {
            TaskData.Add("task_title", $"Task {randomString}");
            TaskData.Add("task_description", $"Task is {randomString}");
            TaskData.Add("email_owner", "maksymiv@test.com");
            TaskData.Add("email_assign", "sdsad@gmail.ru");
        }

        [When(@"I send POST request with task information")]
        public void WhenISendPOSTRequestWithTaskInformation()
        {
            RestRequest request = new RestRequest("/tasks/rest/createtask", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(TaskData);
            response = client.Execute(request);
        }

        [Then(@"I get task id")]
        public void ThenIGetTaskId()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual("success", json["type"].ToString());
            Assert.IsNotNull(json["id_task"].ToString());
        }

        [Given(@"Generated user data with task description")]
        public void GivenGeneratedUserDataWithTaskDescription()
        {
            UserWithTaskData.Add("name", $"Name{randomString}");
            UserWithTaskData.Add("email", "sdsad@gmail.rur");
            UserWithTaskData.Add("tasks", new Dictionary<string, object>() {
                {"title", $"Title {randomString}" },
                {"description", randomString }
            });
        }

        [When(@"I send REST request with user and task description")]
        public void WhenISendRESTRequestWithUserAndTaskDescription()
        {
            RestRequest request = new RestRequest("/tasks/rest/createuserwithtasks", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(UserWithTaskData);
            response = client.Execute(request);
        }

        [Then(@"I get user name and task titles I assigned")]
        public void ThenIGetUserNameAndTaskTitlesIAssigned()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual($"Name{randomString}", json["name"].ToString());
            Assert.AreEqual($"Title {randomString}", json["tasks"][0]["title"].ToString());
        }

        [When(@"I send REST request with my avatar")]
        public void WhenISendRESTRequestWithMyAvatar()
        {
            string email = "maksymiv@test.com";
            RestRequest request = new RestRequest("/tasks/rest/addavatar/?email=" + email, Method.POST);
            request.AddFile("avatar", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"C:\Users\mcsymiv\Downloads\photo5460908032348236852.jpg"));
            response = client.Execute(request);
        }

        [Then(@"I get ok status")]
        public void ThenIGetOkStatus()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual("ok", json["status"].ToString());
        }

        [When(@"I send REST request to delete avatar")]
        public void WhenISendRESTRequestToDeleteAvatar()
        {
            string email = "maksymiv@test.com";
            RestRequest request = new RestRequest("/tasks/rest/deleteavatar?email=" + email, Method.POST);
            response = client.Execute(request);
        }

        [Then(@"I get status ok")]
        public void ThenIGetStatusOk()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual("ok", json["status"].ToString());
        }

        [When(@"I send REST with (.*) in query")]
        public void WhenISendRESTWithInQuery(string query)
        {
            RestRequest request = new RestRequest("/tasks/rest/magicsearch, Method.POST");
            request.RequestFormat = DataFormat.Json;
            Dictionary<string, object> QueryBody = new Dictionary<string, object>()
            {
                { "query", query},
                { "fullSimilarity", true}
            };
            request.AddJsonBody(QueryBody);
            response = client.Execute(request);
        }

        [Then(@"I get (.*) in results")]
        public void ThenIGetInResults(string companyNameResult)
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            Assert.AreEqual(companyNameResult, json["results"][0]["name"].ToString());
        }
    }
}
