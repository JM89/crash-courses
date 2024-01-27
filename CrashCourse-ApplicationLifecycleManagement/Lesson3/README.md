# Lesson 3: Continuous Deployment

TODO: Introduction

## Step 1: Setting up a Octopus Deploy instance

We will set up Octopus Deploy in this lesson but many continuous deployment tools are available. The terminology between these tools might differ, but they essentially work similarly. We will need to configure how to "deploy" your artifacts to your environment. 

To simulate a Octopus Deploy server, we will use once again docker. The containers we will use:
- Octopus Server (based on Windows instance)
- A Linux instance with dotnet runtime so we can simulate the deployment into an environment and run our application
- TeamCity setup (instance + agent): that we will integrate to Octopus at the end of the lesson

Similarly to TeamCity, agents (workers) can be separately configured to deal with your deployments. Without them, you won't be able to parallelize your deployments. In our lesson, we will keep it simple.

After running the docker-compose, you can access Octopus at the following location http://localhost:8080/.

After entering your admin credentials (that you can find in the docker-compose file), you should have the following screen:

![](images/01.PNG)

Click on "Create your first environment" as suggested:

![](images/02.PNG)

And create a "Test" environment and a "Live" environment: 

![](images/03.PNG)

If you check Library > Lifecycle, you can see that a default lifecycle including your two new environments is defined. With lifecycle, you can create a chain of release between your environments. 

![](images/48.PNG)

Now, back to creating the deployment targets. We will only create the Test environment the steps would be similar for Live. 

Click on "Create your first deployment target (optional)" as suggested:

![](images/08.PNG)

Add Deployment Target, select Linux:

![](images/09.PNG)

To keep it simple, we are going to connect to a deployment target using SSH.

This requires some additional setup so let's take a step back here.

First, the docker-compose started 3 containers. 

We will be connecting Octopus server to the docker container `test-environment`. This is our deployment target.

![](images/16.PNG)

Feel free to check the Dockerfile in ./test-environment/Dockerfile, we extend the dotnet runtime base image (ubuntu) and make sure to allow SSH communication. 

Let's create a SSH key pair on `test-environment` by connecting to the docker container: 

`docker container exec -it <container-id> sh`

The instructions are described:
- `ssh-keygen -m PEM` (accept default location & no passphrase)
- `cat ~/.ssh/id_rsa.pub >> ~/.ssh/authorized_keys`
- `chmod 600 ~/.ssh/authorized_keys`

For the sake of simplicity, I'll be using the root account. 

![](images/12.PNG)

Then retrieve the content of the file `cat ~/.ssh/id_rsa` and save it into an `octopus.pem` key.

Back in Octopus, go to Infrastructure > Account, Select SSH Key pair:

![](images/10.PNG)

Then populate the following information:

![](images/11.PNG)

Back to "Deployment Targets", select SSH Connection:

![](images/09.PNG)

And populate the information:
![](images/13.PNG)

Next:
![](images/14.PNG)

You should now have an healthy target: 

![](images/15.PNG) 

## Step 2: Configure the Blog Post API Project

We will configure this in several steps, but let's define a minute project first. 

First, click on the "Projects" menu:

![](images/04.PNG)

"Add Projects":

![](images/05.PNG)

Then:

![](images/06.PNG)

Your project should now be created:

![](images/07.PNG)

Set environment variables:

![](images/48.PNG)

In the menu "Process", Add step and create the following bash script:

![](images/34.PNG)
![](images/35.PNG)

At this point, we only display a message with an environment variable. 

Now let's create a release to test the setup:

![](images/50.PNG)
![](images/51.PNG)
![](images/52.PNG)
![](images/49.PNG)

## Step 3: Integrate with Teamcity

Start by restoring the backup in the Prep/TeamCity_Backup.zip.

![](images/17.PNG)

Then re-authorize the agent and check that you can still run the build.

From TeamCity, we will connect to Octopus to push our artefacts to its internal package repository. To achieve this, we need an API key. 

In Octopus, go to the Configuration > Users and "Add User":

![](images/18.PNG)

Generate a new API Key:

![](images/19.PNG)

Take note of the API key. 

On the same page, click on "assign them to one or more teams.":

Add Team:

![](images/20.PNG)

Add Service Account for TeamCity server as a member go to User Roles. You will need these two permissions:

![](images/21.PNG)

We will cover the concepts when we met them. 

Back to TeamCity, go to Administration then look for the "Plugins" menu items at the bottom. Click on "Upload plugin zip" and select Prep/Octopus.TeamCity.Plugin.zip.

![](images/23.PNG)

Enable uploaded plugin without restart:

![](images/24.PNG)

Next, go to your Blog Post API project > "Build" > [General Settings](http://localhost:8112/admin/editBuild.html?id=buildType:BlogPostApi_Build), and change the artifact path output location to include a package number (%build.number% refers to auto-increment number for your TC builds). Once we start having multiple package to push to Octopus, you want to be able to identify them:

`CrashCourse-InterProcessCommunication/Lesson4/Final/BlogPost/artefact/ => /outputs/BlogPostApi.%build.number%.zip`

![](images/27.PNG)

We create a "Deploy" configuration here which will aim to integrate CI and CD tool together. TeamCity will just trigger the deployment under the condition that the code is on the default branch, while Octopus responsibility here will to action it. 

![](images/Flow.png)

Create a "Deploy" configuration, select "Manually" (as we do not need a VCS root):

![](images/54.PNG)
![](images/53.PNG)

In the "Deploy" configuration,

Add Parameters:

![](images/25.PNG)
![](images/26.PNG)

Create a dependency artifact between your Build & Deploy configuration:
![](images/31.PNG)
![](images/30.PNG)

This targets the outputs of the Build configuration.

Create a trigger between your Build & Deploy configuration:

![](images/32.PNG)

Create a first build step which is pushing the artifacts to Octopus:

![](images/29.PNG)

--package=BlogPostApi.%dep.BlogPostApi_Build.build.number%.zip

![](images/28.PNG)

Create a second build step which creates a release in Octopus and trigger the deployment on Test:

![](image/46.PNG)
![](images/45.PNG)
![](images/44.PNG)

## Step 4: Release the package

TODO: Update package reference
TODO: update script to deploy the code on the target

![](images/38.PNG)