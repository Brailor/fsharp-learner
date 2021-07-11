import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import * as awsx from "@pulumi/awsx";
import path from "path"
import { getType } from 'mime'

// Create an AWS resource (S3 Bucket)
const siteBucket = new aws.s3.Bucket("my-bucket", {
    website: {
        indexDocument: "index.html"
    }
});

const bucketFiles = ["bundle.js", "index.html"]

for (let file of bucketFiles) {
    const filePath = path.resolve(`./client/build/${file}`)
    new aws.s3.BucketObject(file, {
        bucket: siteBucket,
        source: new pulumi.asset.FileAsset(filePath),
        contentType: getType(filePath) || undefined
    })
}

// Create an S3 Bucket Policy to allow public read of all objects in bucket
// This reusable function can be pulled out into its own module
function publicReadPolicyForBucket(bucketName: string) {
    return JSON.stringify({
        Version: "2012-10-17",
        Statement: [{
            Effect: "Allow",
            Principal: "*",
            Action: [
                "s3:GetObject"
            ],
            Resource: [
                `arn:aws:s3:::${bucketName}/*` // policy refers to bucket name explicitly
            ]
        }]
    })
}

// Set the access policy for the bucket so all objects are readable
new aws.s3.BucketPolicy("bucketPolicy", {
    bucket: siteBucket.bucket, // depends on siteBucket -- see explanation below
    policy: siteBucket.bucket.apply(publicReadPolicyForBucket)
    // transform the siteBucket.bucket output property -- see explanation below
});

exports.websiteUrl = siteBucket.websiteEndpoint; // output the endpoint as a stack output