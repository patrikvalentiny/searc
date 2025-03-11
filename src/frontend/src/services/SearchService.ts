import axios from 'axios';
import { environment } from '../../config/environment';
import { FileDetailsDTO } from '../models/FileDetailsDTO';



export async function GetSearchRequest(query: string): Promise<FileDetailsDTO[]> {
    const baseUrl: string = environment.apiBaseUrl;
    try {
        const response = await axios.get(`${baseUrl}/api/v1/search`, {
            params: { query }
        });
        return response.data;
    } catch (error) {
        console.error('Search error:', error);
        throw error;
    }
}
