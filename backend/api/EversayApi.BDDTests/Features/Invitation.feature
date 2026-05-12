Feature: Invitation



Feature: Invitation API

  Scenario: User accepts a valid invitation
    Given a valid invitation code exists
    When the user accepts the invitation
    Then the API should return 200 OK

  Scenario: User accepts an invalid invitation
    Given an invalid invitation code
    When the user accepts the invitation
    Then the API should return 404 NotFound

  Scenario: User accepts an empty invitation code
    Given an empty invitation code
    When the user accepts the invitation
    Then the API should return 400 BadRequest