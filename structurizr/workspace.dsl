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

            cleaner = container "Cleaner Service" {
                description "Processes raw emails from the dataset, removes headers, and forwards cleaned content."
                technology ".NET Worker Service"
            }

            indexer = container "Indexer Service" {
                description "Indexes cleaned emails into the database."
                technology ".NET Worker Service"
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

                searchController -> emailService "Uses to perform search"
            }

            database = container "Database" {
                description "Stores cleaned and indexed emails."
                technology "PostgreSQL"
            }

            ui = container "Web UI" {
                description "Allows users to search emails."
                technology "Razor Pages"
            }

            user -> ui "Searches emails"
            ui -> api "Sends search requests"
            api -> database "Reads email data"
            cleaner -> filesystem "Reads raw emails"
            cleaner -> indexer "Sends cleaned emails"
            indexer -> database "Stores emails"
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
        }

        component enronSystem.api component_diagram {
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
        }

        theme default
    }

}