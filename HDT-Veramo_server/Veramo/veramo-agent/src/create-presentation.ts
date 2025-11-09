import { agent } from "./veramo/setup.js";

async function main() {
    const identifiers = await agent.didManagerFind()

    const credentials = await agent.dataStoreORMGetVerifiableCredentials({
        where:[{column:'issuer', value:[identifiers[2].did]}]
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

        console.log(JSON.stringify(presentation, null, 2))
    }
}

main().catch(console.log)