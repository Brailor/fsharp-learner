import { createFrontend } from './pulumi/frontend'
import { createBackend } from './pulumi/backend'


const siteBucket = createFrontend()
const server = createBackend("t2.micro")

export = {
    websiteUrl: siteBucket.websiteEndpoint,
    publicIp: server.publicIp,
    publicHostName: server.publicDns
}