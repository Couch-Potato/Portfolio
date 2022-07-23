import {EnvType, load} from "ts-dotenv";

export type Env = EnvType<typeof schema>

export const schema = {
    CONNECTION_STRING: String,
    IS_DEBUG: Boolean,
    REPLICA_SET: String,
    DB_HOSTNAME: String,
    CONTROLLER_CONNECTION:String
}

export let env: Env;

export function getConnectionString(database:String) :string {
    return `${env.CONNECTION_STRING}${database}?authSource=admin${env.IS_DEBUG ? `` :`&replicaSet=${env.REPLICA_SET}`}`
}

export function loadEnv() {
    env = load(schema);
}