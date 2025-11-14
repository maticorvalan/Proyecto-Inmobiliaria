-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 09, 2025 at 09:23 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Table structure for table `auditorias`
--

CREATE TABLE `auditorias` (
  `id` int(11) NOT NULL,
  `usuario` varchar(100) NOT NULL,
  `accion` varchar(100) NOT NULL,
  `fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `detalle` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `auditorias`
--

INSERT INTO `auditorias` (`id`, `usuario`, `accion`, `fecha`, `detalle`) VALUES
(1, 'mati@mail.com', 'Creación Pago', '2025-09-27 16:57:50', 'Contrato ID: 2'),
(2, 'mati@mail.com', 'Creación Pago', '2025-09-27 17:01:47', 'Contrato ID: 2'),
(3, 'mati@mail.com', 'Creación Pago', '2025-09-30 17:58:57', 'Contrato ID: 3'),
(4, 'mati@mail.com', 'Anulacion de Pago', '2025-09-30 18:00:00', 'Pago ID: 1'),
(5, 'mati@mail.com', 'Modificación Pago', '2025-09-30 18:12:52', 'Pago ID: 1'),
(6, 'mati@mail.com', 'Modificación Pago', '2025-09-30 18:13:05', 'Pago ID: 1'),
(7, 'mati@mail.com', 'Modificación Pago', '2025-09-30 18:22:44', 'Pago ID: 1'),
(8, 'mati@mail.com', 'Modificación Pago', '2025-09-30 18:50:42', 'Pago ID: 1'),
(9, 'mati@mail.com', 'Modificación Pago', '2025-09-30 18:50:51', 'Pago ID: 1'),
(10, 'mati@mail.com', 'Modificación Pago', '2025-10-02 16:49:03', 'Pago ID: 1'),
(11, 'mati@mail.com', 'Modificación Pago', '2025-10-02 16:49:16', 'Pago ID: 1'),
(12, 'mati@mail.com', 'Creación Contrato', '2025-10-02 17:42:00', 'Contrato ID: 5'),
(13, 'mati@mail.com', 'Creación Contrato', '2025-10-02 17:43:39', 'Contrato ID: 6'),
(14, 'mati@mail.com', 'Creación Contrato', '2025-10-02 17:45:13', 'Contrato ID: 7'),
(15, 'mati@mail.com', 'Creación Contrato', '2025-10-04 02:31:00', 'Contrato ID: 8'),
(16, 'mati@mail.com', 'Creación Contrato', '2025-10-04 02:36:30', 'Contrato ID: 9'),
(17, 'mati@mail.com', 'Creación Contrato', '2025-10-04 02:37:57', 'Contrato ID: 10'),
(18, 'mati@mail.com', 'Creación Contrato', '2025-10-04 02:43:31', 'Contrato ID: 11'),
(19, 'mati@mail.com', 'Creación Contrato', '2025-10-04 02:46:16', 'Contrato ID: 12'),
(20, 'mati@mail.com', 'Finalización Contrato', '2025-10-04 02:51:02', 'Contrato ID: 1 finalizado por renovación'),
(21, 'mati@mail.com', 'Renovación Contrato', '2025-10-04 02:51:02', 'Nuevo Contrato ID: 13 (renovación del contrato 1)'),
(22, 'mati@mail.com', 'Creación Contrato', '2025-10-04 15:38:58', 'Contrato ID: 14'),
(23, 'mati@mail.com', 'Creación Contrato', '2025-10-04 15:46:03', 'Contrato ID: 15'),
(24, 'mati@mail.com', 'Creación Contrato', '2025-10-04 16:11:56', 'Contrato ID: 16'),
(25, 'mati@mail.com', 'Creación Contrato', '2025-10-04 16:13:55', 'Contrato ID: 17');

-- --------------------------------------------------------

--
-- Table structure for table `contratos`
--

CREATE TABLE `contratos` (
  `id` int(11) NOT NULL,
  `monto` decimal(10,0) NOT NULL,
  `fechaInicio` datetime NOT NULL,
  `fechaFin` datetime NOT NULL,
  `estado` tinyint(1) NOT NULL,
  `idInmueble` int(11) NOT NULL,
  `idInquilino` int(11) NOT NULL,
  `fechaterminacionefectiva` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `contratos`
--

INSERT INTO `contratos` (`id`, `monto`, `fechaInicio`, `fechaFin`, `estado`, `idInmueble`, `idInquilino`, `fechaterminacionefectiva`) VALUES
(2, 50000, '2025-09-20 00:00:00', '2025-09-27 00:00:00', 0, 14, 1, NULL),
(3, 50000, '2025-09-04 00:00:00', '2025-09-16 00:00:00', 1, 17, 5, NULL),
(4, 123123, '2025-10-01 00:00:00', '2025-11-22 00:00:00', 1, 18, 4, NULL),
(5, 123123, '2025-10-02 00:00:00', '2025-10-04 00:00:00', 0, 15, 2, NULL),
(13, 123123, '2025-10-05 00:00:00', '2025-11-04 00:00:00', 0, 15, 2, NULL),
(14, 50000, '2025-10-04 00:00:00', '2025-10-05 00:00:00', 0, 14, 6, '2025-10-04 00:00:00'),
(15, 3123123, '2025-12-04 00:00:00', '2026-03-05 00:00:00', 0, 14, 3, '2025-10-04 00:00:00'),
(16, 50000, '2026-05-07 00:00:00', '2026-07-18 00:00:00', 1, 17, 5, NULL),
(17, 3123123, '2025-10-04 00:00:00', '2025-10-10 00:00:00', 0, 17, 1, '2025-10-04 00:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `imagenes`
--

CREATE TABLE `imagenes` (
  `id` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `url` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `imagenes`
--

INSERT INTO `imagenes` (`id`, `InmuebleId`, `url`) VALUES
(2, 14, '/Uploads/Inmuebles\\4f3eb8f2-fc88-4d64-baf0-cb3c1f3bcf51.jpg'),
(3, 15, '/Uploads/Inmuebles\\fc205dce-6098-4c63-b2e5-9f6cbc7ebc22.jpg'),
(9, 17, '/Uploads/Inmuebles\\eacd790f-3b64-40fe-a53b-b635b39992b1.jpg'),
(10, 17, '/Uploads/Inmuebles\\dd6869e9-1e69-4ae0-9ad2-3495896ce979.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `inmuebles`
--

CREATE TABLE `inmuebles` (
  `id` int(11) NOT NULL,
  `direccion` varchar(50) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `uso` varchar(30) NOT NULL,
  `tipo` varchar(100) NOT NULL,
  `superficie` int(11) NOT NULL,
  `latitud` decimal(10,0) NOT NULL,
  `longitud` decimal(10,0) NOT NULL,
  `estado` varchar(30) NOT NULL,
  `precio` int(11) NOT NULL,
  `urlPortada` varchar(500) DEFAULT NULL,
  `idPropietario` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inmuebles`
--

INSERT INTO `inmuebles` (`id`, `direccion`, `ambientes`, `uso`, `tipo`, `superficie`, `latitud`, `longitud`, `estado`, `precio`, `urlPortada`, `idPropietario`) VALUES
(14, 'Calle False 122', 3, 'Comercial', 'Local', 80, 102, 104, 'Ocupado', 350000, '/Uploads/Inmuebles/portada_14.jpg', 9),
(15, 'Uruguay 512', 2, 'Comercial', 'Depósito', 150, 206, 305, 'Ocupado', 800000, '/Uploads/Inmuebles/portada_15.jpg', 3),
(17, 'Super Calle 1300', 4, 'Residencial', 'Casa', 50, 76, 52, 'Ocupado', 450000, '/Uploads/Inmuebles/portada_17.jpg', 5),
(18, 'Calle trucha', 1, 'Comercial', 'Local', 111, 1111, 1111, 'Disponible', 1000000, NULL, 7);

-- --------------------------------------------------------

--
-- Table structure for table `inquilinos`
--

CREATE TABLE `inquilinos` (
  `id` int(11) NOT NULL,
  `DNI` int(11) DEFAULT NULL,
  `nombre` varchar(50) DEFAULT NULL,
  `apellido` varchar(50) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `estado` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `inquilinos`
--

INSERT INTO `inquilinos` (`id`, `DNI`, `nombre`, `apellido`, `telefono`, `email`, `estado`) VALUES
(1, 40125698, 'Laura', 'Ramírez', '1178901234', 'laura.ramirez@gmail.com', 1),
(2, 38965412, 'Diego', 'Sánchez', '1189012345', 'diego.sanchez@gmail.com', 1),
(3, 41237895, 'Sofía', 'Torres', '1190123456', 'sofia.torres@gmail.com', 1),
(4, 39541236, 'Matías', 'Domínguez', '1101234567', 'matias.dominguez@gmail.com', 1),
(5, 42369874, 'Julieta', 'Acosta', '1112345678', 'julieta.acosta@gmail.com', 1),
(6, 231232132, 'Santi', 'dsadsad', '266423123', 'Matias.1222@gmail.com', 0);

-- --------------------------------------------------------

--
-- Table structure for table `pagos`
--

CREATE TABLE `pagos` (
  `id` int(11) NOT NULL,
  `idcontrato` int(11) NOT NULL,
  `fechapago` date NOT NULL,
  `monto` decimal(10,2) NOT NULL,
  `estado` varchar(50) DEFAULT NULL,
  `detalle` varchar(255) DEFAULT NULL,
  `multa` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pagos`
--

INSERT INTO `pagos` (`id`, `idcontrato`, `fechapago`, `monto`, `estado`, `detalle`, `multa`) VALUES
(1, 2, '2025-09-27', 50000.00, 'Cancelado', 'Monto pagado', 0),
(2, 2, '2025-09-27', 50000.00, 'Pagado', 'Monto Aprobado', 0),
(3, 3, '2025-09-30', 50000.00, 'Pendiente', 'Monto pendiente', 0),
(4, 2, '2025-10-04', 50000.00, 'Pagado', 'Pago de multa por terminación anticipada', 1),
(5, 13, '2025-10-04', 246246.00, 'Pagado', 'Pago de multa por terminación anticipada', 1),
(6, 14, '2025-10-04', 100000.00, 'Pagado', 'Pago de multa por terminación anticipada', 1),
(7, 15, '2025-10-04', 6246246.00, 'Pagado', 'Pago de multa por terminación anticipada', 1),
(8, 17, '2025-10-04', 6246246.00, 'Pagado', 'Pago de multa por terminación anticipada', 1);

-- --------------------------------------------------------

--
-- Table structure for table `propietarios`
--

CREATE TABLE `propietarios` (
  `id` int(11) NOT NULL,
  `DNI` int(11) DEFAULT NULL,
  `nombre` varchar(50) DEFAULT NULL,
  `apellido` varchar(50) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `estado` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `propietarios`
--

INSERT INTO `propietarios` (`id`, `DNI`, `nombre`, `apellido`, `telefono`, `email`, `estado`) VALUES
(2, 37775452, 'Pio Ricardo', 'Baroja', '2657545454', 'pio@gmail.com', 1),
(3, 44565245, 'Matias', 'Corvalan', '2664525742', 'marcoteca@hotmail.com', 1),
(4, 30456987, 'Carlos', 'Gómez', '1123456789', 'carlos.gomez@gmail.com', 1),
(5, 28975643, 'María', 'Fernández', '1134567890', 'maria.fernandez@gmail.com', 1),
(6, 31547896, 'Jorge', 'Pérez', '1145678901', 'jorge.perez@gmail.com', 1),
(7, 27654321, 'Ana', 'Martínez', '1156789012', 'ana.martinez@gmail.com', 1),
(8, 33214567, 'Ricardo', 'López', '1167890123', 'ricardo.lopez@gmail.com', 1),
(9, 39137714, 'Angel', 'Baroja', '2664525742', 'Angel@gmail.com', 1);

-- --------------------------------------------------------

--
-- Table structure for table `usuarios`
--

CREATE TABLE `usuarios` (
  `id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Email` varchar(150) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Avatar` varchar(255) DEFAULT '',
  `Rol` int(11) NOT NULL
) ;

--
-- Dumping data for table `usuarios`
--

INSERT INTO `usuarios` (`id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Avatar`, `Rol`) VALUES
(1, 'matias', 'Corvalan', 'mati@mail.com', 'mNLJknwWofoK8VUR9eTtXHQaADBK1+HMST7ZGCwcC4M=', '/Uploads/Avatares\\avatar_1_f2732759-cec5-4782-8d16-a3ecf4f5750b.jpg', 1),
(2, 'angel', 'baroja', 'angel@mail.com', 'zEfYY5GuCHYxJnjvXMW26fucSSSTVhSAhqs7G93KraY=', '/Uploads/Avatares\\avatar_2_ad3fbf3d-ea79-4ef9-b46e-09dbb7f3e6f8.jpg', 1),
(3, 'Santi', 'Bene', 'ANGEL@BAROJA.COM', 'zEfYY5GuCHYxJnjvXMW26fucSSSTVhSAhqs7G93KraY=', '', 2),
(4, 'Pepe', 'Pampa', 'pepe@mail.com', 'mNLJknwWofoK8VUR9eTtXHQaADBK1+HMST7ZGCwcC4M=', '/Uploads/Avatares\\avatar_4_8a908d4a-dc02-4433-aab7-7cf6f2b3dec1.jpg', 2);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `auditorias`
--
ALTER TABLE `auditorias`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idInmueble` (`idInmueble`),
  ADD KEY `idInquilino` (`idInquilino`);

--
-- Indexes for table `imagenes`
--
ALTER TABLE `imagenes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indexes for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idPropietario` (`idPropietario`);

--
-- Indexes for table `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`id`),
  ADD KEY `fk_pago_contrato` (`idcontrato`);

--
-- Indexes for table `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `auditorias`
--
ALTER TABLE `auditorias`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT for table `contratos`
--
ALTER TABLE `contratos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `imagenes`
--
ALTER TABLE `imagenes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT for table `pagos`
--
ALTER TABLE `pagos`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`idInmueble`) REFERENCES `inmuebles` (`id`),
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`idInquilino`) REFERENCES `inquilinos` (`id`);

--
-- Constraints for table `imagenes`
--
ALTER TABLE `imagenes`
  ADD CONSTRAINT `imagenes_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`id`);

--
-- Constraints for table `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`idPropietario`) REFERENCES `propietarios` (`id`);

--
-- Constraints for table `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `fk_pago_contrato` FOREIGN KEY (`idcontrato`) REFERENCES `contratos` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
