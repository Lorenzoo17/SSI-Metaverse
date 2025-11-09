import { agent } from "./veramo/setup.js"

async function main() {
    const identifiers = await agent.didManagerFind()

    // console.log("First identifier : " + identifiers[2].did)

    // Retrieve credential for each DID created
    for(let i = 0; i < identifiers.length; i++){
        const credentials = await agent.dataStoreORMGetVerifiableCredentials({
            where:[{column:'subject', value:[identifiers[i].did]}]
        })

        console.log(credentials[0].verifiableCredential) // print the first credential

        let credentials_string = ""
        for(let j = 0; j < credentials.length; j++){
            credentials_string += JSON.stringify(credentials[j].verifiableCredential) + "\n"
        }

        // console.log(credentials_string)
    
        // console.log("Credential corresponding to " + identifiers[i].alias + " = " + credentials.length)
        if(credentials.length > 0){
            // console.log(credentials[0].verifiableCredential.type)
            const result = await agent.verifyCredential({
                credential: credentials[0].verifiableCredential
            })
            // console.log("Credential verified : " + result.verified)
        }
    }
}

main().catch(console.log)