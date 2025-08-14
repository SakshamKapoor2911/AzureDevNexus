import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../store';
import { fetchPipelines } from '../store/slices/pipelinesSlice';
import Card from '../components/Card';
import Button from '../components/Button';
import LoadingSpinner from '../components/LoadingSpinner';
import { CogIcon, PlayIcon, CheckCircleIcon, XCircleIcon, ClockIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';

const Pipelines: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { pipelines, isLoading, error } = useSelector((state: RootState) => state.pipelines);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  useEffect(() => {
    dispatch(fetchPipelines());
  }, [dispatch]);

  const filteredPipelines = pipelines.filter(pipeline => {
    const matchesSearch = pipeline.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         pipeline.projectName.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesStatus = statusFilter === 'all' || 
                         (statusFilter === 'running' && pipeline.status === 'running') ||
                         (statusFilter === 'success' && pipeline.lastRunResult === 'succeeded') ||
                         (statusFilter === 'failed' && pipeline.lastRunResult === 'failed');
    
    return matchesSearch && matchesStatus;
  });

  const getStatusIcon = (status: string, result?: string) => {
    if (status === 'running') {
      return <PlayIcon className="h-5 w-5 text-blue-600" />;
    }
    
    if (result === 'succeeded') {
      return <CheckCircleIcon className="h-5 w-5 text-green-600" />;
    } else if (result === 'failed') {
      return <XCircleIcon className="h-5 w-5 text-red-600" />;
    } else {
      return <ClockIcon className="h-5 w-5 text-gray-600" />;
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

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <div className="text-red-600 mb-4">Error loading pipelines: {error}</div>
        <Button onClick={() => dispatch(fetchPipelines())}>
          Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Pipelines</h1>
          <p className="text-gray-600">Monitor and manage your Azure DevOps pipelines</p>
        </div>
        <Button>
          New Pipeline
        </Button>
      </div>

      {/* Filters and Search */}
      <Card>
        <div className="flex flex-col sm:flex-row gap-4">
          <div className="flex-1">
            <div className="relative">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <input
                type="text"
                placeholder="Search pipelines..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-azure-500 focus:border-azure-500"
              />
            </div>
          </div>
          
          <div className="flex gap-2">
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-azure-500 focus:border-azure-500"
            >
              <option value="all">All Status</option>
              <option value="running">Running</option>
              <option value="success">Success</option>
              <option value="failed">Failed</option>
            </select>
          </div>
        </div>
      </Card>

      {/* Pipelines List */}
      <div className="space-y-4">
        {filteredPipelines.map((pipeline) => (
          <Card key={pipeline.id} className="hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between p-6">
              <div className="flex items-center space-x-4">
                <div className="flex-shrink-0 h-12 w-12">
                  <div className="h-12 w-12 rounded-lg bg-purple-100 flex items-center justify-center">
                    <CogIcon className="h-7 w-7 text-purple-600" />
                  </div>
                </div>
                
                <div className="flex-1">
                  <div className="flex items-center space-x-3">
                    <h3 className="text-lg font-semibold text-gray-900">{pipeline.name}</h3>
                    <span className="text-sm text-gray-500">•</span>
                    <span className="text-sm text-gray-500">{pipeline.projectName}</span>
                  </div>
                  
                  <div className="mt-1 flex items-center space-x-4">
                    <span className="text-sm text-gray-600">Type: {pipeline.type}</span>
                    <span className="text-sm text-gray-600">Status: {pipeline.status}</span>
                    {pipeline.lastRunDate && (
                      <span className="text-sm text-gray-600">
                        Last run: {new Date(pipeline.lastRunDate).toLocaleDateString()}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              
              <div className="flex items-center space-x-4">
                <div className="flex items-center space-x-2">
                  {getStatusIcon(pipeline.status, pipeline.lastRunResult)}
                  <span className={`inline-flex px-3 py-1 text-sm font-semibold rounded-full ${getStatusColor(pipeline.status, pipeline.lastRunResult)}`}>
                    {getStatusText(pipeline.status, pipeline.lastRunResult)}
                  </span>
                </div>
                
                <div className="flex space-x-2">
                  <Button variant="outline" size="sm">
                    View Details
                  </Button>
                  <Button size="sm">
                    Run Pipeline
                  </Button>
                </div>
              </div>
            </div>
          </Card>
        ))}
      </div>

      {filteredPipelines.length === 0 && (
        <Card className="text-center py-12">
          <CogIcon className="mx-auto h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No pipelines found</h3>
          <p className="text-gray-500 mb-4">
            {searchTerm || statusFilter !== 'all'
              ? 'Try adjusting your search or filters'
              : 'Get started by creating your first pipeline'}
          </p>
          {!searchTerm && statusFilter === 'all' && (
            <Button>Create Pipeline</Button>
          )}
        </Card>
      )}
    </div>
  );
};

export default Pipelines;
