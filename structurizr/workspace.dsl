workspace "Searc" "C4 model of the Searc email search system" {

    !identifiers hierarchical

    model {
        user = person "User" {
            description "A user who searches and downloads emails."
        }

        filesystem = softwareSystem "Raw Email Data (Filesystem)" {
            description "Stores the raw Enron dataset in text files."
        }

        enronSystem = softwareSystem "Searc System" {
            description "Processes, indexes, and provides search over the Enron email dataset."

            # Databases
            searchDatabase = container "Search Database" "PostgreSQL" {
                description "Stores searchable indexed email data."
                technology "PostgreSQL"
                tags "Database"
            }

            indexDatabase = container "Indexer Database" "PostgreSQL" {
                description "Stores indexed words, occurrences, and file references."
                technology "PostgreSQL"
                tags "Database"
            }

            # Message Queues
            cleanedQueue = container "Cleaned Email Queue" "RabbitMQ" {
                description "Message queue for cleaned emails from Cleaner to Indexer."
                technology "RabbitMQ"
                tags "Queue"
            }

            indexedQueue = container "Indexed Data Queue" "RabbitMQ" {
                description "Message queue for indexed data from Indexer to Search API."
                technology "RabbitMQ"
                tags "Queue"
            }

            # Processing Services
            cleaner = container "Cleaner Service" ".NET Worker Service" {
                description "Removes headers from raw emails and publishes cleaned content."
                technology ".NET Worker Service"
                
                cleanerService = component "CleanerService" {
                    description "Processes and cleans raw email files."
                    technology "C# Service"
                }

                messagePublisher = component "CleanedMessagePublisher" {
                    description "Publishes cleaned emails to RabbitMQ with topic 'CleanedFile'."
                    technology "EasyNetQ"
                }
            }

            indexer = container "Indexer Service" ".NET Worker Service" {
                description "Indexes cleaned emails into the database."
                technology ".NET Worker Service"
                
                indexerService = component "IndexerService" {
                    description "Processes cleaned emails and indexes words."
                    technology "C# Service"
                }

                repository = component "IndexerRepository" {
                    description "Handles database operations for indexing words, occurrences, and files."
                    technology "Dapper + PostgreSQL"
                }

                messageHandler = component "IndexedFileHandler" {
                    description "Consumes cleaned email messages and triggers indexing."
                    technology "EasyNetQ"
                }

                messagePublisher = component "IndexedMessagePublisher" {
                    description "Publishes indexed email data to RabbitMQ."
                    technology "EasyNetQ"
                }
            }

            # API Services
            searchAPI = container "Search API" "ASP.NET Core Web API" {
                description "Handles search queries and retrieves results from the database."
                technology "ASP.NET Core Web API"
                
                searchController = component "SearchController" {
                    description "Processes search requests from the frontend."
                    technology "C# Controller"
                }

                searchService = component "SearchService" {
                    description "Handles search logic and interacts with the repository."
                    technology "C# Service"
                }

                searchRepository = component "SearchRepository" {
                    description "Performs database queries for search operations."
                    technology "Dapper + PostgreSQL"
                }

                indexedFileHandler = component "IndexedFileDTOHandler" {
                    description "Consumes indexed email data and updates the search database."
                    technology "EasyNetQ"
                }
            }

            # Frontend
            ui = container "Web UI" {
                description "Allows users to search for emails."
                technology "React + ShadCN"
            }

            # Relationships - Ensuring Clear Flow
            user -> ui "Searches emails"
            ui -> searchAPI "Sends search queries"
            searchAPI -> searchDatabase "Retrieves search results"

            cleaner.cleanerService -> filesystem "Reads raw emails"
            cleaner.cleanerService -> cleanedQueue "Publishes cleaned emails"
            cleaner.messagePublisher -> cleanedQueue "Publishes messages"

            indexer.messageHandler -> cleanedQueue "Consumes cleaned emails"
            indexer.indexerService -> indexDatabase "Stores indexed words, occurrences, and files"
            indexer.messagePublisher -> indexedQueue "Publishes indexed data"

            searchAPI.indexedFileHandler -> indexedQueue "Consumes indexed data"
            searchAPI.searchService -> searchDatabase "Updates search data"
        }
    }

    views {
        systemContext enronSystem system_context {
            include *
            autolayout lr
        }

        container enronSystem container_diagram {
            include *
            autolayout tb
            description "Structured flow of the Searc system from top to bottom."
        }

        component enronSystem.cleaner cleaner_component_diagram {
            include *
            autolayout lr
            description "Detailed view of the Cleaner Service components."
        }

        component enronSystem.indexer indexer_component_diagram {
            include *
            autolayout lr
            description "Detailed view of the Indexer Service components."
        }

        component enronSystem.searchAPI searchapi_component_diagram {
            include *
            autolayout lr
            description "Detailed view of the Search API components."
        }

        styles {
            element "Person" {
                background #ba1e75
                shape person
            }
            element "Software System" {
                background #d92389
            }
            element "Container" {
                background #f8289c
            }
            element "Component" {
                background #f572c1
            }
            element "Database" {
                shape cylinder
            }
            element "Queue" {
                shape pipe
                background #6b9ae5
                color #ffffff
            }
        }

        theme default
    }
}