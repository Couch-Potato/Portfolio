import { Client } from "./auth_client.js";
import express from "express";
/**
 * A dummy class for intellisense that doesn't actually do anything.
 */
export class DummyRequest {
    /**
     * @type {Client}
     */
    Client;

    query;
    params;
}