pipeline {
    agent any
     triggers {
         // Poll every minute just for simulation purpose
        pollSCM('* * * * *') 
    }
    environment {
        PROJECT_NAME = "SampleCIdemo"
        STAGING_ENV = "staging"
        PROD_ENV = "production"
    }

    stages {
        stage('Build') {
            steps {
                echo "Building the project for ${env.PROJECT_NAME}..."
                // Simulate build
                bat 'echo Build completed successfully.'
            }
        }

        stage('Unit and Integration Tests') {
            steps {
                echo 'Running unit and integration tests...'
                bat 'echo All tests passed.'
            }
        }

        stage('Code Analysis') {
            steps {
                echo 'Analyzing code...'
                bat 'echo Code quality is acceptable.'
            }
        }

        stage('Security Scan') {
            steps {
                echo 'Performing security scan...'
                bat 'echo No vulnerabilities found.'
            }
        }

        stage('Deploy to Staging') {
            steps {
                echo "Deploying to ${env.STAGING_ENV} environment..."
                bat 'echo Deployed to staging successfully.'
            }
        }

        stage('Integration Tests on Staging') {
            steps {
                echo 'Running integration tests on staging...'
                bat 'echo Integration tests passed on staging.'
            }
        }

        stage('Deploy to Production') {
            steps {
                input message: 'Approve deployment to production?'
                echo "Deploying to ${env.PROD_ENV}..."
                bat 'echo Deployed to production successfully.'
            }
        }
    }
}
