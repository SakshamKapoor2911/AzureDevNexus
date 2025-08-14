import React from 'react';
import { Pipeline } from '../../store/slices/pipelinesSlice';
import Card from '../Card';
import { CogIcon, PlayIcon, CheckCircleIcon, XCircleIcon, ClockIcon } from '@heroicons/react/24/outline';
import { Link } from 'react-router-dom';

interface PipelineStatusProps {
  pipelines: Pipeline[];
}

const PipelineStatus: React.FC<PipelineStatusProps> = ({ pipelines }) => {
  const getStatusIcon = (status: string, result?: string) => {
    if (status === 'running') {
      return <PlayIcon className="h-4 w-4 text-blue-600" />;
    }
    
    if (result === 'succeeded') {
      return <CheckCircleIcon className="h-4 w-4 text-green-600" />;
    } else if (result === 'failed') {
      return <XCircleIcon className="h-4 w-4 text-red-600" />;
    } else {
      return <ClockIcon className="h-4 w-4 text-gray-600" />;
    }
  };

  const getStatusColor = (status: string, result?: string) => {
    if (status === 'running') {
      return 'bg-blue-100 text-blue-800';
    }
    
    if (result === 'succeeded') {
      return 'bg-green-100 text-green-800';
    } else if (result === 'failed') {
      return 'bg-red-100 text-red-800';
    } else {
      return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusText = (status: string, result?: string) => {
    if (status === 'running') {
      return 'Running';
    }
    
    if (result === 'succeeded') {
      return 'Succeeded';
    } else if (result === 'failed') {
      return 'Failed';
    } else {
      return 'Not Run';
    }
  };

  return (
    <Card
      header={
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-semibold text-gray-900">Pipeline Status</h3>
          <Link
            to="/pipelines"
            className="text-sm text-azure-600 hover:text-azure-700 font-medium"
          >
            View All
          </Link>
        </div>
      }
    >
      <div className="space-y-4">
        {pipelines.slice(0, 5).map((pipeline) => (
          <div key={pipeline.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
            <div className="flex items-center space-x-3">
              <div className="flex-shrink-0 h-8 w-8">
                <div className="h-8 w-8 rounded-lg bg-purple-100 flex items-center justify-center">
                  <CogIcon className="h-5 w-5 text-purple-600" />
                </div>
              </div>
              <div>
                <div className="text-sm font-medium text-gray-900">
                  {pipeline.name}
                </div>
                <div className="text-xs text-gray-500">
                  {pipeline.projectName}
                </div>
              </div>
            </div>
            
            <div className="flex items-center space-x-2">
              {getStatusIcon(pipeline.status, pipeline.lastRunResult)}
              <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(pipeline.status, pipeline.lastRunResult)}`}>
                {getStatusText(pipeline.status, pipeline.lastRunResult)}
              </span>
            </div>
          </div>
        ))}
        
        {pipelines.length === 0 && (
          <div className="text-center py-8">
            <CogIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No pipelines</h3>
            <p className="mt-1 text-sm text-gray-500">Get started by creating a new pipeline.</p>
          </div>
        )}
      </div>
    </Card>
  );
};

export default PipelineStatus;
