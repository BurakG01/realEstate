﻿using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace realEstate.Common.Domain.Model
{
 
    public class City
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId _Id { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Town> Towns { get; set; }
    }
    public class Town
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
