USE Import;

CREATE TABLE FileDetail
(
    FileName VARCHAR(255) PRIMARY KEY,
    TotalRegistries bigint NOT NULL,
    RegistriesImported bigint NOT NULL,
    ImportDate Date NOT NULL
);