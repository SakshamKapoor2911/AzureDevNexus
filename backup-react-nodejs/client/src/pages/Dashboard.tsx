import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../store';
import { fetchProjects } from '../store/slices/projectsSlice';
import { fetchPipelines } from '../store/slices/pipelinesSlice';
import Card from '../components/Card';
import LoadingSpinner from '../components/LoadingSpinner';
import DashboardOverview from '../components/dashboard/DashboardOverview';
import RecentActivity from '../components/dashboard/RecentActivity';
import ProjectStatus from '../components/dashboard/ProjectStatus';
import PipelineStatus from '../components/dashboard/PipelineStatus';

const Dashboard: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { projects, isLoading: projectsLoading } = useSelector((state: RootState) => state.projects);
  const { pipelines, isLoading: pipelinesLoading } = useSelector((state: RootState) => state.pipelines);

  useEffect(() => {
    dispatch(fetchProjects());
    dispatch(fetchPipelines());
  }, [dispatch]);

  if (projectsLoading || pipelinesLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">Welcome to AzureDevNexus - Your Azure DevOps Management Platform</p>
      </div>

      {/* Overview Cards */}
      <DashboardOverview projects={projects} pipelines={pipelines} />

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Project Status */}
        <div className="lg:col-span-2">
          <ProjectStatus projects={projects} />
        </div>

        {/* Pipeline Status */}
        <div>
          <PipelineStatus pipelines={pipelines} />
        </div>
      </div>

      {/* Recent Activity */}
      <RecentActivity />
    </div>
  );
};

export default Dashboard;
