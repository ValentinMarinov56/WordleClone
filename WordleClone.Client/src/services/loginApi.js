import axios from "axios";

const API_URL = import.meta.env.VITE_API_BASE_URL ?? ""; // if the environment variable is not set, default to an empty string, in this case this mean relative path to the current domain, which is the same as the API is hosted on the same domain as the client.

export async function login(credentials) {
    const { data } = await axios.post(`${API_URL}/api/Login/authenticate`, credentials);

    if (data.token) {
        localStorage.setItem("token", data.token);
    }

    return data;
}

/* Previous fetch-based implementation:
import { apiRequest } from "./client";

export function login(credentials) {
    return apiRequest("/api/Login/authenticate", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(credentials)
    });
}
*/
