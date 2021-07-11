import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
export function createBackend(instanceType = 't2.micro') {
    const ami = pulumi.output(aws.ec2.getAmi({
        filters: [
            {
                name: "name",
                values: ["amzn-ami-hvm-*"]
            }
        ],
        owners: ["137112412989"],
        mostRecent: true
    }))

    const group = new aws.ec2.SecurityGroup("webserver-secgrp", {
        ingress: [
            { protocol: "tcp", fromPort: 22, toPort: 22, cidrBlocks: ["0.0.0.0/0"] },
            { protocol: "tcp", fromPort: 80, toPort: 80, cidrBlocks: ["0.0.0.0/0"] },
        ]
    })

    // use docker instead of manually do these kind of things
    const userData =
        `#!/bin/bash
        sudo yum install aspnetcore-runtime-5.0
    `

    const server = new aws.ec2.Instance("webserver-www", {
        instanceType,
        vpcSecurityGroupIds: [group.id],
        ami: ami.id
    })

    return server
}