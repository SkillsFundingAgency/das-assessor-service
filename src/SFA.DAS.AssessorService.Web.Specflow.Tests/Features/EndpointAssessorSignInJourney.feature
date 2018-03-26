@signin journey
Feature: EPAO Sign In Journey as a registered user and verifying if forgot password link is working
As an EPAO
I want to be able to sign in onto idAMS portal with my email address or User ID & Password
So that I can access my account
	
Scenario: ON-83 Test .1 Scenario 1 - User click on start button and taken to idAMS page
Given I have already registered on idAMS 
And I'm on the record end point assessment outcome page
When I click on start now button on the record end point assessment outcome page
Then I should be taken onto idMAS sign in page

Scenario:  ON-83 Test .2 Scenario 2 User logs in with valid credentials and redirected to search for apprentice page
Given User Logs on through idams
And User enters valid credentials 
When Clicks on sign in button
Then User should be navigated to search for an apprentice page on EPAO service

Scenario:  ON-83 Test .3 Scenario 2 User Cannot remember Password
Given User Logs on through idams
When User Selects forgot my password
Then User should be navigated to forgotten password screen

Scenario:  ON-83 Test .4 Scenario 2 User Does not have an account
Given User Logs on through idams
When User Selects dont have an account
Then User should be navigated to dont have an account screen

