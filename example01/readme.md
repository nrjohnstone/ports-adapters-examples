# Hexagonal Architecture (aka Ports and Adapters) #

## Example 01 ##

Transitioning from thinking about Software Architecture in the traditional N-Tier sense to patterns such as Clean Architecture, Onion Architecture and Hexagonal Architecture can be a bit of a mental hurdle.

I've been using Hexagonal Architecture as my go to pattern for implementations in the .NET space since around 2015, and one of the things I wished I had available to me back then was a simple reference that demonstrates the concepts clearly.

## What This Repository is NOT ##

### Production ready code ###

Please do not take this code and copy and paste it into a production solution without considering the following

* failure modes
* concurrency
* idempotency
* logging
* metrics

and all the other things that should be in anyones MVP before the put an application into production.
This code strives to keep itself focused on how the inside of an application, the core business logic, can be seperated from the outside concerns of databases, message buses and email severs etc... and as such, the above points are either ignored or glossed over very quickly.

### The One Best Way ###

This example by no means claims to be the one best way of implementing hexagonal architecture, but over time and working with a number of developers I find it allows the quickest path to 'groking' ports, adapters and inside / outside thinking.

## The Sample Space ##

The example uses a book order service as the domain, and the use cases that are implemented are ;

* Allow users to submit book title order requests from a supplier which are aggregated into a single book order for that supplier
* Allow users to approve or reject a book order 
* Allow users to send a book order to a supplier for purchase and shipping

The work flow consists of book title orders being submitted, at some point a book order is approved and then the book order is sent
to the supplier.

Multiple customers want to run the book ordering service but each has a slightly different configuration requirements

### Client 1 ###

* wants to use a REST endpoint to allow their client applications to submit book title orders
* wants to use a REST endpoint to allow their client applications to approve book title orders
* wants to use a REST endpoint to allow their client applications to send book title orders
* wants to use email to notify their book wholesaler of their orders when sending
* wants to store their state in a MySql relational database

### Client 2 ###

* wants to use a message queue (RabbitMq) to allow their client applications to submit book title orders
* wants to use a REST endpoint to allow their client applications to approve book title orders
* wants to use a REST endpoint to allow their client applications to send book title orders
* wants to use a message queue (RabbitMq) to notify their book wholesaler of their orders when sending
* wants to store their state in a MySql relational database

### Client 3 ###

* wants to poll a folder at regular intervals for a delimted file matching an expression and use the content to submit book title orders
* wants to use a REST endpoint to allow their client applications to approve book title orders
* wants to use a REST endpoint to allow their client applications to send book title orders
* wants to use a message queue (RabbitMq) to notify their book wholesaler of their orders when sending
* wants to store their state in a CouchDb document database

## Testing ##
This repository also contains various examples of unit and integration testing.

I have deliberately moved away from using mocking frameworks in unit tests for persistence interfaces as I 
believe the over use of mocks for the following reasons
* Maintaining an in memory implementation for each interface forces you to consider the port/interface if you only have
a single implementation (eg you only support SQL server)
* Mocking frameworks are too forgiving in the face of signature changes and will often keep a unit test passing where it should be failing. 
  By having another implementation to maintain it means when you change interface contracts you are more likely to be forced to update the tests correctly.
* Using in memory implementations forces your tests to become more "data driven" as you have to populate them first in a sane manner
rather than setting up mock expectations with arcane syntax requirements
  
