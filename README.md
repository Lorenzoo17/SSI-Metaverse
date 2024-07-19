# UNISS-METAVERSE

## Description of the system

The system implemented can be described as a simulation of a metaverse in which multiple users can interact with each other. Once the user has registered to the HDT system they can access the Metaverse.
They are required to insert username and password of their account and, once logged in correctly, they can decide to start a new session (become the host of a new Metaverse room) or join an already existying one.
The HDT system comes with a SSI agent which manages all the information of the user. In particular, the agent communicates with the Metaverse and lets users to manage their Verifiable Credentials (VCs), Decentralized Identities (DIDs) and Verifiable Presentations (VP). 
Once the user has logged in, looking at their hand they can access a menu which lets the user see:
1) Their HDT avatar together with the data retrieved from a cloud platform which is linked to the wearable sensors the user is currently wearing;
2) A list of all their sensible data, stored as Verifiable Credentials;
3) Information about their personal account (DID, username, ect...).

Users can communicate with each other exchanging their Verifiable credentials thanks to the SSI agent associated with the Metaverse application. They can request a specific type of credential and at the same time they can share the fields of the requested credential through the Selective Disclosure mechanism. Such a mechanism lets users to be sovereign of their data, selecting only the fields that they actually want to share with the remote user that made the request.  
## How to use