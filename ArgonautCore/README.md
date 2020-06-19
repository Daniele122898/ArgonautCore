# Functional Wrappers and Helpers
<a href="https://www.nuget.org/packages/ArgonautCore">
    <img alt="Argonaut Core" src="https://img.shields.io/nuget/vpre/ArgonautCore.svg?maxAge=2592000?style=plastic">
</a>

This includes functional types that help building more robust and provable functions. The usage is pretty
self explanatory from the XML documentations of the actual functions and types.

### Error handling
I also prefer to use `Result` or `Maybe` to propagate errors instead of throwing exceptions everytime. Exceptions in my mind are for exceptional cases, when shit hits the fan. Not when some API call might return an error that it often does. For that i prefer Go's error handling in just returning an error object that can be handled. 

<br>

### There are two types of wrappers:

## Lightweight (Lw)
The lightweight implementation are struct and thus stack based. These are for fast instancing and avoiding GC. These are also more complete with multiple types like `Some`, `None`, `Option`, `Result`. 
For further understanding read the XML documentation in the source :)

**ATTENTION:** Don't use this if the object gets created somewhere but used much further down or up the stack since this will copy the object multiple times. Use `Maybe` for this.

## Maybe
This is useful for cases where the object might hold an error that gets propagated way up. This way only the reference is passed instead of the object copied. Otherwise use the Lw options.