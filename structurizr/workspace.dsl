workspace "Searc" "C4 model of the Searc email search system" {

    !identifiers hierarchical

    model {
        user = person "User" {
            description "A user who searches and downloads emails."
        }

        filesystem = softwareSystem "Local Filesystem" {
            description "Stores the raw Enron dataset."
        }

        enronSystem = softwareSystem "Searc System" {
            description "Processes, indexes, and provides search over the Enron email dataset."

            # Message broker
            messageBus = container "Email Queue" "RabbitMQ message bus for raw emails" {
                description "Handles asynchronous communication between cleaner and indexer services."
                technology "RabbitMQ"
                tags "Queue"
            }
            
            indexDataQueue = container "Index Data Queue" "RabbitMQ message bus for indexed data" {
                description "Handles asynchronous communication between indexer and data API services."
                technology "RabbitMQ"
                tags "Queue"
            }
            
            # Database containers
            indexDatabase = container "Index Database" {
                description "Stores indexed email data."
                technology "PostgreSQL"
                tags "Database"
            }
            
            searchDatabase = container "Search Database" {
                description "Stores email data optimized for search queries."
                technology "PostgreSQL"
                tags "Database"
            }

            # Processing services
            cleaner = container "Cleaner Service" {
                description "Processes raw emails from the dataset, removes headers, and forwards cleaned content."
                technology ".NET Worker Service"
                
                emailReader = component "EmailReader" {
                    description "Reads raw email files from the filesystem"
                    technology "C# File I/O"
                }
                
                contentCleaner = component "ContentCleaner" {
                    description "Removes headers and normalizes email content"
                    technology "C# Text Processing"
                }
                
                messagePublisher = component "MessagePublisher" {
                    description "Publishes cleaned emails to the message queue"
                    technology "RabbitMQ Client"
                }
            }

            indexer = container "Indexer Service" {
                description "Indexes cleaned emails into the database."
                technology ".NET Worker Service"
                
                messageConsumer = component "MessageConsumer" {
                    description "Consumes cleaned email messages from the queue"
                    technology "RabbitMQ Client"
                }
                
                emailParser = component "EmailParser" {
                    description "Parses email content into structured data"
                    technology "C# Text Processing"
                }
                
                indexWriter = component "IndexWriter" {
                    description "Writes parsed data to the index database"
                    technology "Entity Framework Core"
                }
                
                dataPublisher = component "DataPublisher" {
                    description "Publishes indexed data to the Index Data Queue"
                    technology "RabbitMQ Client"
                }
                
                dataExporter = component "DataExporter" {
                    description "Exports indexed data to the Database API"
                    technology "HTTP Client"
                }
            }

            # API services
            dataAPI = container "Data API" {
                description "Processes indexed data and updates search database."
                technology ".NET Worker Service"
                
                dataConsumer = component "DataConsumer" {
                    description "Consumes indexed data messages from the queue"
                    technology "RabbitMQ Client"
                }
                
                dataTransferService = component "DataTransferService" {
                    description "Manages data transfer between databases"
                    technology "C# Service"
                }
                
                dataMapperService = component "DataMapperService" {
                    description "Maps data between different database schemas"
                    technology "AutoMapper"
                }
            }

            api = container "Search API" {
                description "Handles search requests."
                technology "ASP.NET Core Web API"

                searchController = component "SearchController" {
                    description "Handles search queries from the UI."
                    technology "ASP.NET Core MVC Controller"
                }

                emailService = component "EmailService" {
                    description "Encapsulates the search logic and database access."
                    technology "C# Service"
                }
            }

            # User interface
            ui = container "Web UI" {
                description "Allows users to search emails."
                technology "Razor Pages"
                
                searchPage = component "SearchPage" {
                    description "Displays the search interface"
                    technology "Razor Page"
                }
                
                resultsComponent = component "ResultsComponent" {
                    description "Displays search results"
                    technology "Razor Component"
                }
                
                emailViewComponent = component "EmailViewComponent" {
                    description "Displays individual email content"
                    technology "Razor Component"
                }
            }

            # Internal component relationships
            cleaner.emailReader -> cleaner.contentCleaner "Passes raw emails to"
            cleaner.contentCleaner -> cleaner.messagePublisher "Sends cleaned content to"
            
            indexer.messageConsumer -> indexer.emailParser "Passes cleaned emails to"
            indexer.emailParser -> indexer.indexWriter "Sends parsed data to"
            indexer.indexWriter -> indexer.dataPublisher "Sends indexed data to"
            
            dataAPI.dataConsumer -> indexDataQueue "Consumes messages from"
            dataAPI.dataConsumer -> dataAPI.dataTransferService "Passes indexed data to"
            dataAPI.dataTransferService -> dataAPI.dataMapperService "Uses"
            dataAPI.dataTransferService -> searchDatabase "Writes transformed data to"
            
            api.searchController -> api.emailService "Uses to perform search"
            
            ui.searchPage -> ui.resultsComponent "Displays"
            ui.resultsComponent -> ui.emailViewComponent "Navigates to"

            # Container relationships
            user -> ui "Searches emails"
            ui -> api "Sends search requests"
            api -> searchDatabase "Reads email data"
            
            cleaner.emailReader -> filesystem "Reads raw emails from"
            cleaner.messagePublisher -> messageBus "Publishes messages to"
            
            indexer.messageConsumer -> messageBus "Consumes messages from"
            indexer.indexWriter -> indexDatabase "Writes indexed data to"
            indexer.dataPublisher -> indexDataQueue "Publishes indexed data to"
            
            # High-level container relationships for the diagram
            cleaner -> messageBus "Publishes cleaned emails"
            messageBus -> indexer "Delivers cleaned emails"
            indexer -> indexDatabase "Stores indexed emails"
            indexer -> indexDataQueue "Publishes indexed data"
            indexDataQueue -> dataAPI "Delivers indexed data"
            dataAPI -> searchDatabase "Updates search data"
        }
    }

    views {
        systemContext enronSystem system_context {
            include *
            autolayout lr
        }

        container enronSystem container_diagram {
            include *
            autolayout lr
            description "Shows the container diagram for the Searc system with data flow from left to right"
        }

        component enronSystem.api component_diagram {
            include *
            autolayout lr
        }
        
        component enronSystem.cleaner cleaner_component_diagram {
            include *
            autolayout lr
        }
        
        component enronSystem.indexer indexer_component_diagram {
            include *
            autolayout lr
        }
        
        component enronSystem.dataAPI dataapi_component_diagram {
            include *
            autolayout lr
        }
        
        component enronSystem.ui ui_component_diagram {
            include *
            autolayout lr
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