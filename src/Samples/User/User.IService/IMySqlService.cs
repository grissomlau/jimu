using Jimu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using User.IService.dto;

namespace User.IService
{
    /// <summary>
    /// try mysql
    /// </summary>
    [JimuServiceRoute("/{Service}")]
    public interface IMySqlService : IJimuService
    {
        /*
        copy bellow my sql to create database
        =====================================


CREATE DATABASE jimu_sample;
USE jimu_sample;

CREATE TABLE `User`(
Id INT  PRIMARY KEY auto_increment,
`Name` VARCHAR(64) NULL,
`Email` VARCHAR(64) null
);

CREATE TABLE `Order`(
Id int PRIMARY KEY auto_increment,
`TotalAmount` DECIMAL(20,2) NOT null DEFAULT 0 ,
`CreateTime` DATETIME NOT NULL DEFAULT current_timestamp
);

CREATE TABLE `UserFriend`(
Id int PRIMARY KEY auto_increment,
`UserId` VARCHAR(64) ,
`FriendUserId` VARCHAR(64)
);
         */



        /// <summary>
        /// get all user
        /// </summary>
        /// <returns></returns>
        [JimuGet(true)]
        List<UserModel> GetAllUser();

        [JimuGet(true)]
        Task<UserModel[]> GetAllUserArray();

        /// <summary>
        ///  get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [JimuGet(true)]
        UserModel GetUser(int id);

        /// <summary>
        /// add user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [JimuPost(true)]
        int AddUser(UserModel user);

    }
}
