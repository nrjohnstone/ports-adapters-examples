# Hexagonal Architecture (aka Ports and Adapters) #

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

* Allow users to submit book title orders from a supplier which are aggregated into a single book order
* Allow users to approve a book order
* Allow users to send a book order to a book wholesaler for purchase and shipping

Multiple clients want to run the book ordering service and each has a slightly different configuration

### Client 1 ###

* wants to use a REST endpoint to allow their client applications to submit book title orders
* wants to use a REST endpoint to allow their client applications to approve book title orders
* wants to use a REST endpoint to allow their client applications to send book title orders
* wants to use email to notify their book wholesaler of their orders when sending

### Client 2 ###

* wants to use a message queue (RabbitMq) to allow their client applications to submit book title orders
* wants to use a REST endpoint to allow their client applications to approve book title orders
* wants to use a REST endpoint to allow their client applications to send book title orders
* wants to use a message queue (RabbitMq) to notify their book wholesaler of their orders when sending

* Both clients want to store their state in a MySql database
