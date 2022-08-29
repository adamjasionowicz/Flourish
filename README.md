# Getting Started with Flourish

To get started, you need to get a copy locally. You have two options: fork or clone.

You should **fork this repository** only if you plan on submitting a pull request. Or if you'd like to keep a copy of a snapshot of the repository in your own GitHub account.

You should **clone this repository** if you're one of the contributors and you have commit access to it. Otherwise you probably want one of the other options.

## The Branching Strategy

We use GitHub flow. It works as follows:

1. When a developer starts a new development of either a fix or feature, it creates a new branch from the main branch. The branch’s name must be as descriptive as possible (e.g., new-oauth2-scopes).
2. The developer checks out the newly created branch and begins development.
3. Once development is completed, the developer creates a new pull request to the main branch.
4. Another developer in the team verifies the pull request’s code and approves or rejects it.
5. If approved, the code is then merged into the main branch, triggering eventual CI/CD pipelines and deploying the code directly to production.
   Each branch created from the main branch has a very short life. This small size not only makes it harder to introduce big bugs but also makes it easy to identify and fix them; imagine having to find a bug in a commit of 1000 lines of code in 10 files instead of 100 lines of code in just two files.

## The Core Project

The Core project should include things like:

- Entities
- Aggregates
- Domain Events
- DTOs
- Interfaces
- Event Handlers
- Domain Services
- Specifications

## The SharedKernel Project

Used to share code between multiple bounded contexts.

## The Infrastructure Project

The application's dependencies on external resources are implemented in classes defined in the Infrastructure project. These classes implement interfaces defined in Core. Examples of what lives here: Data access, domain event implementations, email providers, file access, web api clients, etc. The Infrastructure project depends on `Microsoft.EntityFrameworkCore.SqlServer` and `Autofac`.

## The Web Project

The entry point of the application is the ASP.NET Core web project. This includes its configuration system, which uses `appsettings.json` plus environment variables, and is configured in `Startup.cs`. The project delegates to the `Infrastructure` project to wire up its services using Autofac.

The web project contains multiple sandbox examples: MVC, API, Endpoints, ClientApp. This is soley for the purpose of the playground as you would not typically implement multiple entry point types in one solution.

## The Test Projects

In terms of dependencies, there are three worth noting:

- [xunit](https://www.nuget.org/packages/xunit)

- [Moq](https://www.nuget.org/packages/Moq/)

- [Microsoft.AspNetCore.TestHost](https://www.nuget.org/packages/Microsoft.AspNetCore.TestHost) Using TestHost, you make actual HttpClient requests. Tests run in memory and are very fast, and requests exercise the full MVC stack, including routing, model binding, model validation, filters, etc.

# Patterns Used

This solution has code built in to support a few common patterns, especially Domain-Driven Design patterns.

## Domain Events

Domain events are a great pattern for decoupling a trigger for an operation from its implementation. This is especially useful from within domain entities since the handlers of the events can have dependencies while the entities themselves typically do not. In the sample, you can see this in action with the `ToDoItem.MarkComplete()` method. The following sequence diagram demonstrates how the event and its handler are used when an item is marked complete through a web API endpoint.
