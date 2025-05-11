pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                echo 'Building the project...'
                bat 'mvn clean package'
            }
        }

        stage('Unit and Integration Tests') {
            steps {
                echo 'Running unit and integration tests...'
                bat 'mvn test'
            }
        }

        stage('Code Analysis') {
            steps {
                echo 'Analyzing code with SonarQube...'
                withSonarQubeEnv('SonarQubeServer') {
                    bat 'mvn sonar:sonar'
                }
            }
        }

        stage('Security Scan') {
            steps {
                echo 'Running security scan...'
                bat 'dependency-check.sh --project MyApp --scan ./target'
            }
        }

        stage('Deploy to Staging') {
            steps {
                echo 'Deploying to staging...'
                bat 'ansible-playbook deploy-staging.yml'
            }
        }

        stage('Integration Tests on Staging') {
            steps {
                echo 'Running integration tests on staging...'
                bat 'newman run tests.postman_collection.json'
            }
        }

        stage('Deploy to Production') {
            steps {
                input message: 'Approve deployment to production?'
                echo 'Deploying to production...'
                bat 'ansible-playbook deploy-production.yml'
            }
        }
    }
}
