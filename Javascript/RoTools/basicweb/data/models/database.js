import mongoose from "mongoose";

const database = new mongoose.Schema({
    database_name: String, 
    data: Object,
    index: String,
})



database.statics.getDatabase = async function(name) {
    var dataset =  this;
    return  {
        getById : async (id) =>{
            return await dataset.findById(id);
        },
        getByIndex: async (index) =>{
            return await dataset.findOne({database_name:name, index:index});
        },
        query: async(query, options)=>{
            var limit = options.limit ? options.limit : 100;
            var adjQ = {}
            for (var i of query) {
                adjQ["data." + i] = query[i];
            }
            var query = dataset.find(adjQ).limit(limit);
            if (options.select)
                query.select(options.select)
            if (options.sort) 
                query.sort(options.sort)

            return await query.exec();
        },
        queryOne: async (query, options) => {
            var limit = 1;
            var adjQ = {}
            for (var i of query) {
                adjQ["data." + i] = query[i];
            }
            var query = dataset.find(adjQ).limit(limit);
            if (options.select)
                query.select(options.select)
            if (options.sort)
                query.sort(options.sort)

            var res = await query.exec();
            if (res)
                return res[0];
        },
        getByData: async (data) =>{
            return await dataset.findOne({data:data});
        }
    }
}




export default mongoose.model("Database", database);