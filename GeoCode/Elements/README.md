# Introduction

This directory is the one with most of the code using Microstation's SDK.
You can find examples in the SDK folder, but as they are either hard to set-up,
deprecated or not clear at all (sometimes all the three).  
I'll try my best to help you understand the SDK by giving tips or explanations here or in comments.

*\*From now, I may refer to Microstation as MSTN.*

## The 2 APIs

The First thing you need to know is that there is not one but two APIs in this SDK,
and the second is just calling the first one.

The one you are using is **CONNECT**. It's the new C# API, and it's the one we're using here.  
The other one is **NATIVE**. It's the original C / C++ API, used by the major part of MSTN's developers and is the one
called by CONNECT under the hood.

You can also directly call NATIVE API while using CONNECT through the Bentley.Interop Name space, but I highly recommend
you NOT TO DO IT because the two APIs don't work the same way at all, and using Interop one time will lead into using
it anywhere.