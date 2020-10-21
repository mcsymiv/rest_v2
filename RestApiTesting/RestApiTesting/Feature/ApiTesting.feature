Feature: ApiTestSteps
	In order to manage my compary activities
	As a user of Bugred website
	I want to update my company information

	Background: 
		Given A new client is created

Scenario: Create new client
	Given A new client is created
	And A user data is generated
	When I send user credentials to the site
	Then I get response with user information

Scenario: Successfully created company
	Given A company data is generated
	When I sent POST request with company data
	Then I get response contain Company test name name

Scenario: Successfuly created user
	Given Generated user infomation
	When I send POST request with user data
	Then I get back user information in the response body

Scenario: Assign a task to the user
	Given A task data is generated
	When I send POST request with task information
	Then I get task id

Scenario: Create a user with assigned task
	Given Generated user data with task description
	When I send REST request with user and task description
	Then I get user name and task titles I assigned

Scenario: A User add avatar
	When I send REST request with my avatar
	Then I get ok status

Scenario: A user deletes avatar
	When I send REST request to delete avatar
	Then I get status ok

Scenario: A user make company search
	When I send REST with Company test name name in query
	Then I get Company test name in results