# Fluent Validation in ASP.NET Core 3 – Powerful Validations


When it comes to Validating Models, aren’t we all leaning towards Data Annotations? There are quite a lot of serious issues with this approach for a scalable system. There is a library, Fluent Validations that can turn up the validation game to a whole new level, giving you total control.

In this repo, we will talk about Fluent Validation and it’s implementation in ASP.NET Core Applications. We will discuss the preferred alternative to Data Annotations and implement it in an ASP.Net core API

--------------------------------

The Problem

Data Validation is extremely vital for any Application. The GO-TO Approach for Model validation in any .NET Application is Data Annotations, where you have to declare attributes over the property of models. Worked with it before? 

```ruby

public class Developer
    {
       [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Range(minimum:5,maximum:20)]
        public decimal Experience { get; set; }
    }


```
It is fine for beginners. But once you start learning clean code, or begin to understand the SOLID principles of application design, you would just never be happy with Data Annotations as you were before. It is clearly not a good approach to combine your models and validation logic.

With the implementation of Data Annotations in .NET Classes, the problem is that there will be a lot of duplication lines of code throughout your application. What if the Developer Model class is to used in another application/method where these Attribute validation changes? What if you need to validate a model that you don’t have access to? Unit testing can get messy as well. You will definitely end up build multiple model classes which will no longer be maintainable in the longer run. So, what’s the solution?


# Introducing Fluent Validation – The Solution

Fluent Validation is a free to use .NET validation library that helps you make your validations clean, easy to create, and maintain. It even works on external models that you don’t have access to, with ease. With this library, you can separate the model classes from the validation logic like it is supposed to be. It doesn’t make your Model classes ugly like Data Annotations does. Also, better control of validation is something that makes the developers prefer Fluent Validation.

For small systems, I would recommend just using Data Annotations, because they’re so easy to set up. For larger, more complex systems, I would recommend separating the validation concern using validator objects with Fluent Validation.


Implementing Fluent Validation in ASP.NET Core Applications

For this simple demonstration, let’s work on an ASP.NET Core 3.1 API Project that does nothing other than just validation with Fluent Validation. I will be using Postman to test and receive the validation messages and Visual Studio 2019 Community as my IDE ( the best for C# development)
Getting Started

//Here is how the project structure would look like.

# Installing FluentValidation.AspNetCore
Begin by installing this awesome library into your WebApi project via the Package Manage Console.

```ruby
Install-Package FluentValidation.AspNetCore

```

Configuring FluentValidation

We will have to add Fluent Validation to our application. Navigate to Startup.cs and modify as follows.


```ruby

public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(s => 
                { 
                    s.RegisterValidatorsFromAssemblyContaining<Startup>(); 
                    s.RunDefaultMvcValidationAfterFluentValidationExecutes = false; 
                });
        }

```

Line 4 Add the Fluent Validation.
Line 6 Registers all the Custom Validations that are going to build. Note that, we will place our Validators within the API Project for this demonstration.
Line 7 It is possible to use both Fluent Validation and Data Annotation at a time. Let’s only support Fluent Validation for now

# Developer model
```ruby

public class Developer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal Experience { get; set; }
    }


```
create a controller 
we will just send a response OK if the passed model is valid.

Remeber we spoke about validators while registering the services at Starup.cs?


```ruby

[HttpPost]
        public async Task<IActionResult> Create(Developer developer)
        {
            return Ok();
        }


```

# What’s a Custom Fluent Validator?

In order to apply custom validation rules to properties of a model class / object. we will have to build custom validators. These Abstract Validators are of type T, where T will be your concerned class.
Creating the First Validator

Since we have made the Developer Model, let’s create a validator with 1 or 2 rules, just to get started.



```ruby
//note we can pass string localizor in the ctor if we want to localize the validation message 
public class DeveloperValidator : AbstractValidator<Developer>
    {
        public DeveloperValidator()
        {
            RuleFor(X => X.FirstName).NotEmpty().WithMessage("required_first_name");
        }
    }


```
# Testing with Postman

Run your API and open up Postman and POST a Developer model to the endpoint we built just now, ../api/developer.



```ruby


{
	"FirstName":"mahmoud",
	"LastName":"khalifa",
	"Email":"hello@cwm.com",
	"Experience":4
}

```

All good, we get an OK message. Now, remove the FirstName and leave it blank.Send a POST request.

then try to remove first name ---> will get bad request and the error message

now we can add another validation rule like string length


```ruby

RuleFor(X => X.FirstName)
                .NotEmpty()
                .WithMessage("required_first_name")
                .Length(2,10).WithMessage("first_name_length_should_be_between_2_and_10");

```
We get multiple validation error messages now. Pretty Sweet! But on a practical point of view, do you really need to show these 2 errors? Like, they are almost meaning the same. So how do we avoid showing 2 validation errors?


```ruby



```



# Integrating Custom Functions
As as example, Theoretically you could pass a number as the first name and the API would just say ‘Cool, that’s fine’. Let’s see how to build a validation rule with Fluent Validation for such a scenario.

Firsty create a simple helper method that would take in our firstname property and return a boolean based on it’s content. We will place this method in our DeveloperValidator class itself.


```ruby

private bool IsValidName(string name)
        {
            return name.All(Char.IsLetter);
        }
        
RuleFor(X => X.FirstName)
                .NotEmpty()
                .WithMessage("required_first_name")
                .Length(2,10).WithMessage("first_name_length_should_be_between_2_and_10")
                .Must(IsValidName).WithMessage("first_name_should_be_characters_only");

```
when must() pass first name to the IsValidName function then check if it's valid or not
or simply we can use in line  approach if the rule is simple

```ruby

 RuleFor(X => X.FirstName)
                .NotEmpty()
                .WithMessage("required_first_name")
                .Length(2,10).WithMessage("first_name_length_should_be_between_2_and_10")
                .Must(x=>x.All(char.IsLetter)).WithMessage("first_name_should_be_characters_only");


```

* [there are more built in validation] (https://docs.fluentvalidation.net/en/latest/built-in-validators.html)


Manual Validation

There can be a case where you need to validate an object manually within your application using FLuent Validation. 
Let’s try to replicate such an use-case. We will build a response model in Models/ResponseModel.cs which will return a list of error message from within our code.


```ruby

public class ResponseModel
    {
        public ResponseModel()
        {
            IsValid = true;
            ValidationMessages = new List<string>();
        }
        public bool IsValid { get; set; }
        public List<string> ValidationMessages { get; set; }
    }


```


```ruby

public class Tester
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal Experience { get; set; }
    }


```





```ruby

 public class TesterValidator : AbstractValidator<Tester>
    {
        public TesterValidator()
        {
            RuleFor(p => p.FirstName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("{PropertyName} should be not empty. NEVER!")
                .Length(2, 25);
        }
    }


```





```ruby

[HttpPost]
        public async Task<IActionResult> Create()
        {
            TesterValidator validator = new TesterValidator();
            List<string> ValidationMessages = new List<string>();
            var tester = new Tester
            {
                FirstName = "",
                Email = "bla!"
            };
            var validationResult = validator.Validate(tester);
            var response = new ResponseModel();
            if (!validationResult.IsValid)
            {
                response.IsValid = false;
                foreach (ValidationFailure failure in validationResult.Errors)
                {
                    ValidationMessages.Add(failure.ErrorMessage);
                }
                response.ValidationMessages = ValidationMessages;
            }
            return Ok(response);
        }


```

# Summary

I hope you have understood this simple to follow guide about Fluent Validation. 
I am sure you will be switching to this powerful library today onwards! 
These guys have some great documentation as well. Check it out too! 
