CREATE SCHEMA IF NOT EXISTS public;

CREATE TABLE public.words (
    id SERIAL PRIMARY KEY,
    word TEXT NOT NULL
);

CREATE TABLE public.files(
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    content BYTEA NOT NULL
);

CREATE TABLE occurrences(
    word_id INT NOT NULL,
    file_id INT NOT NULL,
    count INT NOT NULL,
    PRIMARY KEY (word_id, file_id),
    FOREIGN KEY (word_id) REFERENCES public.words(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (file_id) REFERENCES public.files(id) ON DELETE CASCADE ON UPDATE CASCADE
);
