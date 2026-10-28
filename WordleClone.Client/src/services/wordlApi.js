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

export function getWordls() {
    return axios.get(`${API_URL}/api/Wordl`).then(response => response.data);
}

export function getWordlById(id) {
    return axios.get(`${API_URL}/api/Wordl/${id}`).then(response => response.data);
}

export function createWordl(wordlData) {
    return axios.post(`${API_URL}/api/Wordl`, wordlData, getAuthConfig())
        .then(response => response.data);
}

export function deleteWordl(id) {
    return axios.delete(`${API_URL}/api/Wordl/${id}`, getAuthConfig())
        .then(response => response.data);
}

/* Previous fetch-based implementation:
import { apiRequest, apiRequestWithAuth } from "./client";

export function getWordls() {
    return apiRequest("/api/Wordl");
}

export function getWordlById(id) {
    return apiRequest(`/api/Wordl/${id}`);
}

export function createWordl(wordlData) {
    return apiRequestWithAuth("/api/Wordl", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(wordlData)
    });
}

export function deleteWordl(id) {
    return apiRequestWithAuth(`/api/Wordl/${id}`, {
        method: "DELETE"
    });
}
*/