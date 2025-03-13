import axios from 'axios';
import { environment } from '../../config/environment';

export async function GetDownloadRequest(fileId: number, filename:string): Promise<void> {
    const baseUrl: string = environment.apiBaseUrl;
    try {
        const response = await axios.get(`${baseUrl}/v1/download/${fileId}`, {
            responseType: 'blob'
        });
        
        // Create a URL for the blob
        const blob = new Blob([response.data]);
        const url = window.URL.createObjectURL(blob);
        
        // Create a temporary link and trigger download
        const link = document.createElement('a');
        link.href = url;
        link.download = `file-${filename}`; // You can set a custom filename here
        document.body.appendChild(link);
        link.click();
        
        // Cleanup
        window.URL.revokeObjectURL(url);
        document.body.removeChild(link);
    } catch (error) {
        console.error('Download error:', error);
        throw error;
    }
}