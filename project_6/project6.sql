create database myStore;
go

use myStore;

create table users(
	id int primary key identity(1,1),
	firstName nvarchar(max),
	lastName nvarchar(max),
	email nvarchar(max),
	password nvarchar(max),
	img nvarchar(max),
	address nvarchar(max),
	type nvarchar(max)
);

create table category(
	id int primary key identity(1,1),
	name nvarchar(max),
	description nvarchar(max),
	img nvarchar(max)
);

create table product(
	id int primary key identity(1,1),
	name nvarchar(max),
	description nvarchar(max),
	price decimal(18, 2),
	img_1 nvarchar(max),
	img_2 nvarchar(max),
	img_3 nvarchar(max),
	img_4 nvarchar(max),
	quantity_inStock int,
	category_id int,
	foreign key (category_id) references category(id)
);

create table cart (
	id int primary key identity(1,1),
	user_id int,
	foreign key (user_id) references users(id)
);

create table cart_item(
	id int primary key identity(1,1),
	size nvarchar(max),
	product_id int,
	quantity int,
	cart_id int,
	foreign key (product_id) references product(id),
	foreign key (cart_id) references cart(id)
);

create table transactions(
	id int primary key identity(1,1),
	amount decimal,
	status nvarchar(max),
	paypal_transaction_id nvarchar(max)
)

create table orders (
	id int primary key identity(1,1),
	user_id int,
	transaction_id int,
	foreign key (user_id) references users(id),
	foreign key (transaction_id) references transactions(id)
);


create table order_item(
	id int primary key identity(1,1),
	size nvarchar(max),
	product_id int,
	quantity int,
	order_id int,
	foreign key (product_id) references product(id),
	foreign key (order_id) references orders(id)
);

INSERT INTO category ("name", "description", "img") VALUES 
('TOPS', 'Designed to keep you looking chic and feeling comfortable all day long.', 'tops (1).jpg'),
('BOTTOMS', 'Designed to suit every occasion and style.', 'bottoms.jpg'),
('DRESSES', 'Designed with care and crafted from high-quality fabrics, these dresses combine comfort with effortless elegance.', 'dresses.jpg'),
('KNITS', 'Our knits are designed to keep you cozy while looking effortlessly chic.', 'knits.jpg'),
('ACCESSORY', 'Designed to add that perfect finishing touch to any outfit.', 'accessory.jpg');



INSERT INTO product ("name","description","price","img_1","img_2","img_3","img_4","category_id") VALUES 
('Brown - Crew neck - Tunic','Size of the model: Size: 38, Height: 176 cm, Bust: 82 cm, Waist: 63 cm, Hips: 91 cm', 21.99, 'Brown - Crew neck - Tunic(1).JPG', 'Brown - Crew neck - Tunic(2).JPG', 'Brown - Crew neck - Tunic(3).JPG', 'Brown - Crew neck - Tunic(4).JPG',1),
('Navy Blue - Tunic','Size of the item on the image: Size: 38, Bust: 104 cm, Waist: 90 cm, Hips: 105 cm, Front Size: 110 cm', 12.60, 'Navy Blue-Tunic(1).jpg', 'Navy Blue-Tunic(2).jpg', 'Navy Blue-Tunic(3).jpg', 'Navy Blue-Tunic(4).jpg',1),
('Pink - Blouses','Fabric Info: 90% Polyester, 10% Cotton', 36.29, 'Pink-Blouses(1).jpg', 'Pink-Blouses(2).jpg', 'Pink-Blouses(3).jpg', 'Pink-Blouses(4).jpg',1);



