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

export function getReviews() {
    return axios.get(`${API_URL}/api/Review`).then(response => response.data);
}

export function getReviewById(id) {
    return axios.get(`${API_URL}/api/Review/${id}`).then(response => response.data);
}

export function createReview(reviewData) {
    return axios.post(`${API_URL}/api/Review`, reviewData, getAuthConfig())
        .then(response => response.data);
}

export function deleteReview(id) {
    return axios.delete(`${API_URL}/api/Review/${id}`, getAuthConfig())
        .then(response => response.data);
}

/* Previous fetch-based implementation:
import { apiRequest, apiRequestWithAuth } from "./client";

export function getReviews() {
    return apiRequest("/api/Review");
}

export function getReviewById(id) {
    return apiRequest(`/api/Review/${id}`);
}

export function createReview(reviewData) {
    return apiRequestWithAuth("/api/Review", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(reviewData)
    });
}

export function deleteReview(id) {
    return apiRequestWithAuth(`/api/Review/${id}`, {
        method: "DELETE"
    });
}
*/