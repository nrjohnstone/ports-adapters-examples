create table book_orders (
    order_id CHAR(36) NOT NULL PRIMARY KEY,
    supplier VARCHAR(60) NOT NULL,
    state VARCHAR(60) NOT NULL);  

create table book_order_lines ( 
    order_line_id CHAR(36) NOT NULL PRIMARY KEY,
    order_id CHAR(36) NOT NULL,
    title VARCHAR(60) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    quantity INT NOT NULL
);

create table book_order_line_conflicts (
    id CHAR(36) NOT NULL PRIMARY KEY,
    order_id CHAR(36) NOT NULL,
    order_line_id CHAR(36) NOT NULL,
    conflict_type VARCHAR(32) NOT NULL,
    conflict_value CHAR(36) NOT NULL,
    accepted BOOLEAN NOT NULL
);