import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import Layout from './components/Layout/Layout';
import Login from './pages/Login/Login';
import Dashboard from './pages/Dashboard/Dashboard';
import Projects from './pages/Projects/Projects';
import ProjectDetails from './pages/ProjectDetails/ProjectDetails';
import Pipelines from './pages/Pipelines/Pipelines';
import PipelineDetails from './pages/PipelineDetails/PipelineDetails';
import WorkItems from './pages/WorkItems/WorkItems';
import Repositories from './pages/Repositories/Repositories';
import Settings from './pages/Settings/Settings';
import NotFound from './pages/NotFound/NotFound';
import LoadingSpinner from './components/UI/LoadingSpinner/LoadingSpinner';

// Protected Route component
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

const App: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="App">
      <Routes>
        {/* Public routes */}
        <Route 
          path="/login" 
          element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <Login />} 
        />
        
        {/* Protected routes */}
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <Layout />
            </ProtectedRoute>
          }
        >
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="projects" element={<Projects />} />
          <Route path="projects/:projectId" element={<ProjectDetails />} />
          <Route path="pipelines" element={<Pipelines />} />
          <Route path="pipelines/:pipelineId" element={<PipelineDetails />} />
          <Route path="work-items" element={<WorkItems />} />
          <Route path="repositories" element={<Repositories />} />
          <Route path="settings" element={<Settings />} />
        </Route>

        {/* Catch all route */}
        <Route path="*" element={<NotFound />} />
      </Routes>
    </div>
  );
};

export default App;
