interface EmailResult {
  id: number;
  filename: string;
  content: string;
}

interface FileCardProps {
  result: EmailResult;
}

export const FileCard: React.FC<FileCardProps> = ({ result }) => {
  return (
    <div className="card card-sm bg-base-100 flex-auto basis-1/3">
      <div className="card-body">
        <h2 className="card-title">File: {result.filename}</h2>
        <p className="card-body">{result.content}</p>
        <div className="card-actions">
          <button className="btn btn-primary">
            Download
          </button>
        </div>
      </div>
    </div>
  );
};
