import { agent } from './veramo/setup.js'

import { createRequire } from "module"// In order to use require with import

const require = createRequire(import.meta.url) // Define require
const http = require("http")
const url = require("url")
const host = "localhost"
const port = 8000

const requestListener = async function(req, res){
  res.setHeader("Content-Type", "application/json")
  const parsedUrl = url.parse(req.url, true)
  const pathname = parsedUrl.pathname
  const query = parsedUrl.query

  if(pathname === '/list'){ // Example : curl http://192.168.137.69:8000/list
    res.writeHead(200)

    const identifiers = await agent.didManagerFind()
    let num = identifiers.length

    res.end('{"message": "number of did registered : ' + num + '"')
  }
  else if(pathname === '/create'){ // Example : curl http://192.168.137.69:8000/create?name=test_name
    if(query.name){ 
      res.writeHead(200)

      const identifier = await agent.didManagerCreate({ alias : query.name}) 
      res.end('{"message": "Assigned did : ' + identifier + '"')
    }else{
      res.writeHead(400)

      res.end('{"error": "No name provided"')
    }
  }
  else if(pathname == "/getDID"){
    if(query.name){
      res.writeHead(200)

      const identifiers = await agent.didManagerFind()
      let did_to_return = '-1'
      identifiers.forEach(did => {
        if(did.alias == query.name){
          did_to_return = did.did
        }
      });
      
      res.end(did_to_return)
    }
  }
  else if(pathname === '/resolve'){ // TO COMPLETE!!! -> for now -> Example : curl http://192.168.137.69:8000/resolve?did=did:.....
    if(query.did){
      res.writeHead(200)
      res.end('{"message": "test for resolve : ' + query.did + '"')
    }else{
      res.writeHead(400)
      res.end('{"error": "No DID provided"')
    }
  }
  else if(pathname == '/getVCs'){
    if(query.did){ // or alias
      res.writeHead(200)

      const credentials = await agent.dataStoreORMGetVerifiableCredentials({
        where:[{column:'subject', value:[query.did]}]
      })
      res.end(JSON.stringify(credentials[0].verifiableCredential))
    }
  }
  else if(pathname == '/getVP'){
    if(query.did){
      res.writeHead(200)

      const credentials = await agent.dataStoreORMGetVerifiableCredentials({
        where:[{column:'issuer', value:[query.did]}]
      })

      if(credentials.length > 0){
        const credential = credentials[0].verifiableCredential // take the first credential saved
        // console.log(credential)

        // console.log(String(credential.credentialSubject.id))
        const presentation = await agent.createVerifiablePresentation({
            presentation:{
                verifiableCredentials: credentials,
                holder: String(credential.credentialSubject.id),
                '@context': ['https://www.w3.org/2018/credentials/v1'],
                type: ['VerifiablePresentation'],
            },
            proofFormat: 'jwt',
        })
        res.end(JSON.stringify(presentation, null, 2))
      }
    }
  }
  else{
    res.writeHead(400)
    res.end('{"error": "Request not found"')
  }
  

};

const server = http.createServer(requestListener)
server.listen(port, host, () => {
  console.log('server is running on http://' + host + ":" + port)
});