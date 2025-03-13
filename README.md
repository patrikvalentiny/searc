# searc - Enron Email Dataset Search Engine
A distributed system for cleaning, indexing, and searching the Enron email dataset.

## Introduction
SEARC is a high-performance search system built to process and make searchable the Enron email dataset - a collection of approximately 1.7 GB of email data from the Enron Corporation scandal. This project creates a scalable architecture to clean, index, and provide search functionality for this large text corpus.

## How to use
- Copy the unzipped data into ./data folder so that the the peoples inboxes are the child folders (ex. `data/allen-p`)
- Run the DataPartitioner application to separate the data into folders. The final structure should look like `data/A/allen-p`