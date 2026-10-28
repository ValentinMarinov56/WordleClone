import axios from "axios";

const API_URL = import.meta.env.VITE_API_BASE_URL ?? "";

function getAuthConfig() {
    const token = localStorage.getItem("token");

    if (!token) {
        throw new Error("No authentication token found");
    }

    return {
        headers: {
            Authorization: `Bearer ${token}`
        }
    };
}

export function getUsers() {
    return axios.get(`${API_URL}/api/User`).then(response => response.data);
}

export function getUserById(id) {
    return axios.get(`${API_URL}/api/User/${id}`).then(response => response.data);
}

export function createUser(userData) {
    return axios.post(`${API_URL}/api/User`, userData).then(response => response.data);
}

export function updateUserStats(userData) {
    return axios.put(`${API_URL}/api/User`, userData, getAuthConfig())
        .then(response => response.data);
}

export function deleteUser(id) {
    return axios.delete(`${API_URL}/api/User/${id}`, getAuthConfig())
        .then(response => response.data);
}

/* Previous fetch-based implementation:
import { apiRequest, apiRequestWithAuth } from "./client";

export function getUsers() {
    return apiRequest("/api/User");
}

export function getUserById(id) {
    return apiRequest(`/api/User/${id}`);
}

export function createUser(userData) {
    return apiRequest("/api/User", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(userData)
    });
}

export function updateUserStats(userData) {
    return apiRequestWithAuth("/api/User", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(userData)
    });
}

export function deleteUser(id) {
    return apiRequestWithAuth(`/api/User/${id}`, {
        method: "DELETE"
    });
}
*/

