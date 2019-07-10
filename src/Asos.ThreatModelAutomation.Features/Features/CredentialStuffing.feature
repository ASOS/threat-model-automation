Feature: Some terse yet descriptive text of what is desired
  In order to access a users account
  As an external attacker
  I want to enumerate known passwords
  
  Scenario: Baseline - No Controls
    Given no controls
    When I enumerate known passwords
    Then I gain access access a users account
	
  Scenario: Control 1 - Account Lockout
    Given Locking account after failed logins
    When I enumerate known passwords
    Then I don't gain access access a users account

  Scenario: Control 2 - No Weak Passwords
    Given No account creation with weak passwords
    When I enumerate known passwords
    Then I don't gain access access a users account
