### Introduction

This repository contains an easy-to-use and well-documented .NET assembly for communicating with
an XMPP server. It supports basic Instant Messaging and Presence funtionality as well as a variety
of XMPP extensions.


### Supported XMPP Features

The library fully implements the [XMPP Core](http://xmpp.org/rfcs/rfc3920.html) and 
[XMPP IM](http://xmpp.org/rfcs/rfc3921.html) specifications and thusly provides the basic XMPP instant
messaging (IM) and presence functionality. In addition, the library offers support for most of the
optional procotol extensions. More specifically, the following features are supported:

+ SASL Authentication (PLAIN, DIGEST-MD5, and SCRAM-SHA-1)
+ User Avatars
+ SOCKS5 and In-Band File-Transfer
+ In-Band Registration
+ User Mood
+ User Tune
+ User Activity
+ Simplified Blocking
+ API designed to be very easy to use
+ Well documented with lots of example code
+ Free to use in commercial and personal projects (MIT License)

### Features In Development

The following features are partially supported and are under development. As such, they are mostly untested:

+ XEP-0313 - Message Archive Management
+ XEP-0133 - Service Administration
+ XEP-0136 - Message Archiving
+ XEP-0045 - Multi-User Chat (MUC)


### Credits

The Sharp.Xmpp library is copyright © 2015 Panos Georgiou Stath.
The initial S22.Xmpp library is copyright © 2013-2014 Torben Könke.


### License

This library is released under the [MIT license](https://github.com/pgstath/Sharp.Xmpp/blob/master/License.md).
