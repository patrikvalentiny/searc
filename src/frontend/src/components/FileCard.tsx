import { FileDetailsDTO } from "../models/FileDetailsDTO";

interface FileCardProps {
  result: FileDetailsDTO;
}

export const FileCard: React.FC<FileCardProps> = ({ result }) => {
  return (
    <div className="card card-sm bg-base-100 flex-auto basis-1/3">
      <div className="card-body">
        <h2 className="card-title">File: {result.filename}</h2>
        <div className="card-actions">
          <button className="btn btn-ghost" disabled>
            Download
          </button>
        </div>
      </div>
    </div>
  );
};
