-- MariaDB dump 10.17  Distrib 10.4.14-MariaDB, for Win64 (AMD64)
--
-- Host: 127.0.0.1    Database: messzendzser
-- ------------------------------------------------------
-- Server version	10.4.14-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `chatroom`
--

DROP TABLE IF EXISTS `chatroom`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chatroom` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `icon` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chatroom`
--

LOCK TABLES `chatroom` WRITE;
/*!40000 ALTER TABLE `chatroom` DISABLE KEYS */;
/*!40000 ALTER TABLE `chatroom` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chatroom_associations`
--

DROP TABLE IF EXISTS `chatroom_associations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `chatroom_associations` (
  `user_id` int(11) NOT NULL,
  `chatroom_id` int(11) NOT NULL,
  PRIMARY KEY (`user_id`,`chatroom_id`),
  KEY `user_id_idx` (`user_id`),
  KEY `chatroom_id_idx` (`chatroom_id`),
  CONSTRAINT `chatroom_associations_chatroom_id` FOREIGN KEY (`chatroom_id`) REFERENCES `chatroom` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `chatroom_associations_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chatroom_associations`
--

LOCK TABLES `chatroom_associations` WRITE;
/*!40000 ALTER TABLE `chatroom_associations` DISABLE KEYS */;
/*!40000 ALTER TABLE `chatroom_associations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `image_chat_message`
--

DROP TABLE IF EXISTS `image_chat_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `image_chat_message` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `sent_time` datetime NOT NULL,
  `chatroom_id` int(11) NOT NULL,
  `token` varchar(60) NOT NULL,
  `format` varchar(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `image_chat_message_chatroom_id_idx` (`chatroom_id`),
  KEY `image_chat_message_user_id_idx` (`user_id`),
  CONSTRAINT `image_chat_message_chatroom_id` FOREIGN KEY (`chatroom_id`) REFERENCES `chatroom` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `image_chat_message_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `image_chat_message`
--

LOCK TABLES `image_chat_message` WRITE;
/*!40000 ALTER TABLE `image_chat_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `image_chat_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `text_chat_message`
--

DROP TABLE IF EXISTS `text_chat_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `text_chat_message` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `sent_time` datetime NOT NULL,
  `chatroom_id` int(11) NOT NULL,
  `message` varchar(500) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `user_id_idx` (`user_id`),
  KEY `chatroom_id_idx` (`chatroom_id`),
  CONSTRAINT `text_chat_message_chatroom_id` FOREIGN KEY (`chatroom_id`) REFERENCES `chatroom` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `text_chat_message_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `text_chat_message`
--

LOCK TABLES `text_chat_message` WRITE;
/*!40000 ALTER TABLE `text_chat_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `text_chat_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(60) NOT NULL,
  `username` varchar(60) NOT NULL,
  `password` varchar(90) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (7,'asd2@gmail.com','asd23','AQAAAAEAACcQAAAAEDcixeQkryV+ZT/1Za31Qud6TC31TKi6gY4MLBDL1ZeP7ttnpWZOgzxdY8ZBpJhW'),(8,'asd3@gmail.com','asd234','AQAAAAEAACcQAAAAELYZOSwHnHQ2tu75tJuf1Vi/tYR8gf/zLO7ihdiyKxELS3PWYUsWpx6Puca+qTQF'),(9,'asd4@gmail.com','asd2345','AQAAAAEAACcQAAAAEEflV6K9VhX6JCgzAN3MvjB1HLgeNhmIcZpPn0Qu+iBiTPia/ZXG59rlFqeFdJ0n'),(11,'voip@gmail.com','voip','AQAAAAEAACcQAAAAEKC1qx3gll9/gjaC4tHu2M7q5JDYyaQTrcFoeCrLjvvu7M9zVkY7cTSrfUU8yknt'),(13,'voip1@test.com','voip1','Password1!'),(14,'asdasd@asjodina.comn','asdas','AQAAAAEAACcQAAAAEBj3TlxWgBxs+MaVwDJbzdL1uOEGKl3nlyvsTQHqEOAc1IYIEIVQtOy1NCbQq3xQ'),(15,'testuser@localhost.com','testuser','AQAAAAEAACcQAAAAEDxtTMA34RNkt5oVO31grlKz1ed8AQxhC8CdvfZ3HjQ6XXNjSjc+NYS8mDXuuXTc'),(16,'asdasda@asicna.com','asdasd','AQAAAAEAACcQAAAAECTBlrfSZpPdehW4ZrIaeBOXUNlyoDpWHUzrUucvsB/YWmdeVIv1IJbG+mmFfyRe');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `voice_chat_message`
--

DROP TABLE IF EXISTS `voice_chat_message`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `voice_chat_message` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `sent_time` datetime NOT NULL,
  `chatroom_id` int(11) NOT NULL,
  `token` varchar(60) NOT NULL,
  `length` int(11) NOT NULL,
  `format` varchar(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `voice_chat_message_chatroom_id_idx` (`chatroom_id`),
  KEY `voice_chat_message_user_id_idx` (`user_id`),
  CONSTRAINT `voice_chat_message_chatroom_id` FOREIGN KEY (`chatroom_id`) REFERENCES `chatroom` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `voice_chat_message_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `voice_chat_message`
--

LOCK TABLES `voice_chat_message` WRITE;
/*!40000 ALTER TABLE `voice_chat_message` DISABLE KEYS */;
/*!40000 ALTER TABLE `voice_chat_message` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `voip_credentials`
--

DROP TABLE IF EXISTS `voip_credentials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `voip_credentials` (
  `user_id` int(11) NOT NULL,
  `voip_username` varchar(60) NOT NULL,
  `voip_password` varchar(32) NOT NULL,
  `voip_realm_hash` varchar(32) NOT NULL,
  PRIMARY KEY (`user_id`),
  CONSTRAINT `voip_credentials_user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `voip_credentials`
--

LOCK TABLES `voip_credentials` WRITE;
/*!40000 ALTER TABLE `voip_credentials` DISABLE KEYS */;
INSERT INTO `voip_credentials` VALUES (11,'voip','Password1!','d2e04a4b03af11f8d11d705f63521f46'),(13,'voip1','f08313847640f5fff5e0049349fa43d1','df1b96a81a5e32bd14f8475de0b37d0c'),(14,'asdas','89e9d468b23762c03a0ec6c62a311aa5','225feaf9121e445de2e9dde9dbfa7328'),(15,'testuser','cc7f8970052d4858e124d54c0ad748fe','e16d4f2dd9a39a8518d264fe84df7bdd'),(16,'asdasd','34fb4386b02eedbf75882be207b8bc02','ae515026ffda23f60fae22908f98e0b9');
/*!40000 ALTER TABLE `voip_credentials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `whiteboard`
--

DROP TABLE IF EXISTS `whiteboard`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `whiteboard` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `chatroom_id` varchar(45) NOT NULL,
  `event` varchar(400) NOT NULL,
  `time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `whiteboard`
--

LOCK TABLES `whiteboard` WRITE;
/*!40000 ALTER TABLE `whiteboard` DISABLE KEYS */;
/*!40000 ALTER TABLE `whiteboard` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-10-03 15:10:08

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `register_user`(IN email_in varchar(60),IN username_in varchar(60),IN password_in varchar(80))
BEGIN
    SET @conflict_count = (SELECT count(id) FROM user as u WHERE u.email = email_in);
    IF @conflict_count != 0 THEN
        SIGNAL sqlstate '50001' SET message_text = 'email adress is already taken';
    END IF;
    SET @conflict_count = (SELECT count(id) FROM user as u WHERE u.username = username_in);
    IF @conflict_count != 0 THEN
        SIGNAL sqlstate '50002' SET message_text = 'username adress is already taken';
    END IF;
    START TRANSACTION;
    INSERT INTO user (email, username, password) VALUES(email_in,username_in,password_in);
    SET @new_user_id = (SELECT id FROM user WHERE email = email_in);    
    SET @voip_password = (SELECT MD5(RAND()) as password);    
    INSERT INTO voip_credentials (user_id,voip_username,voip_password,voip_realm_hash) VALUES (@new_user_id,username_in,@voip_password,MD5(CONCAT(username_in,":messzendzser:",@voip_password)));
    COMMIT;
END$$
DELIMITER ;
