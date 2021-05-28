# Ports & Adapters Examples

Transitioning from thinking about Software Architecture in the traditional N-Tier sense to patterns such as Clean Architecture, Onion Architecture and Hexagonal Architecture can be a bit of a mental hurdle.

I've been using Hexagonal Architecture as my go to pattern for implementations in the .NET space since around 2015, and one of the things I wished I had available to me back then was a simple reference that demonstrates the concepts clearly.

The name Ports & Adapters I think gives the clearest picture on how to think about the layout of a solution, with the most import points being where do certain classes live and where do certain interfaces live.

## What This Repository is NOT ##

### Production ready code ###

Please do not take this code and copy and paste it into a production solution without considering the following

* failure modes
* concurrency
* idempotency
* logging
* metrics
* correlation of executions

and all the other things that should be in anyones MVP before the put an application into production.
This code strives to keep itself focused on how the inside of an application, the core business logic, can be seperated from the outside concerns of databases, message buses and email severs etc... and as such, the above points are either ignored or glossed over very quickly.

## Warnings - Here Be Dragons

### The One Best Way ###

How I lay out a .NET solution by no means claims to be the one best way of implementing hexagonal architecture, but over time and mentoring scores of developers I think it's got a lot going for it and also allows the quickest path to 'groking' ports, adapters and inside / outside thinking.

The rules I follow for Ports & Adapters architecture can be summarized as follows ; 

* The interfaces for ports are OWNED by the Domain and as such they live in the Domain assembly. Not another "Something.Interfaces" assembly.

  * You do NOT need to share the interfaces with anything else outside of the solution so DON'T put them in seperate assemblies.
  * Having the ports (interfaces) in the same assembly as the business logic (Domain Assembly) means that everything assembly containing classes that implement a port interface (for example MySql database implementation) has to have a reference to the Domain assembly. The consequence of this means that you cannot add references from the Domain assembly back to an implementation assembly (circular reference)
  * Having the ports in the same assembly as the business logic helps to ensure that implementation details (Adapters) can't leak types into the domain since if the Adapter is referencing the Domain assembly, it is impossible for any public types from the Adapter to be used in the Domain because, obviously, that would be a circular reference.
  * Having the ports in the same assembly as the business logic also makes it fairly obvious that they should NOT be returning types that represent a specific database implementation but rather be returning "Entities" or "Value Types" from the Domain

* Always always always create multiple Host adapters and at least an InMemory persistence adapter. Doing this forces you to make your Domain code generic because it now has to deal with two Hosts (eg a CommandLine console host and a WebApi host) and have a better chance of being able to handle a different implementation should the need arise, and it's not that much work for a very large benefit.
* Do not share bloody DTO classes between different assemblies. You probably want to do it because you believe you are being "DRY" and it currently looks the "same" but all you are doing is now coupling two different things (eg a WebApi and a WPF UI) to the same DTO classes. For the sake of writing a bit of extra mapping code (which is the easiest to write and test, but not the most thrilling I agree) you can have each Host/Adapter with their own DTO types and make them internal to the assembly as well which indicates to everyone that they are an implementation detail.
* Map Map Map ... Across every boundary make sure you are using internal types and map to and from these types when you cross a boundary. The Domain should only ever be passing and recieving types from the Domain assembly, which means all mapping is done in the "adapter" assemblies
* Use the internal keyword ALL THE TIME. Do not make classes public unless they should be used by another assembly.

