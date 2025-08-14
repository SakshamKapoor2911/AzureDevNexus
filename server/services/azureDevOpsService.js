const axios = require('axios');
const logger = require('../utils/logger');

class AzureDevOpsService {
  constructor() {
    this.baseUrl = process.env.AZURE_DEVOPS_ORG_URL;
    this.pat = process.env.AZURE_DEVOPS_PAT;
    this.apiVersion = '7.0';
    
    if (!this.baseUrl || !this.pat) {
      logger.warn('Azure DevOps configuration incomplete - some features may not work');
    }
  }

  /**
   * Create authenticated request headers
   * @returns {Object} Headers object
   */
  getAuthHeaders() {
    if (!this.pat) {
      throw new Error('Azure DevOps PAT not configured');
    }

    return {
      'Authorization': `Basic ${Buffer.from(`:${this.pat}`).toString('base64')}`,
      'Content-Type': 'application/json'
    };
  }

  /**
   * Make a request to Azure DevOps API
   * @param {string} endpoint - API endpoint
   * @param {Object} options - Request options
   * @returns {Promise<Object>} API response
   */
  async makeRequest(endpoint, options = {}) {
    try {
      if (!this.baseUrl) {
        throw new Error('Azure DevOps organization URL not configured');
      }

      const url = `${this.baseUrl}/_apis/${endpoint}`;
      const config = {
        headers: this.getAuthHeaders(),
        ...options
      };

      logger.debug({
        message: 'Making Azure DevOps API request',
        url,
        method: options.method || 'GET'
      });

      const response = await axios(url, config);
      return response.data;

    } catch (error) {
      logger.error({
        message: 'Azure DevOps API request failed',
        endpoint,
        error: error.message,
        status: error.response?.status,
        statusText: error.response?.statusText,
        responseData: error.response?.data
      });

      // Handle specific Azure DevOps error codes
      if (error.response?.status === 401) {
        throw new Error('Unauthorized - check Azure DevOps PAT');
      } else if (error.response?.status === 403) {
        throw new Error('Forbidden - insufficient permissions');
      } else if (error.response?.status === 429) {
        throw new Error('Rate limit exceeded - too many requests');
      } else if (error.response?.status >= 500) {
        throw new Error('Azure DevOps service unavailable');
      }

      throw error;
    }
  }

  /**
   * Get organizations
   * @returns {Promise<Array>} Organizations list
   */
  async getOrganizations() {
    try {
      // Note: This endpoint may not be available with PAT authentication
      // You might need to use Azure AD authentication for this
      const response = await this.makeRequest('projects', {
        params: {
          'api-version': this.apiVersion,
          '$top': 1000
        }
      });

      // Extract organization info from projects
      const orgInfo = {
        id: this.baseUrl.split('/').pop(),
        name: this.baseUrl.split('/').pop(),
        url: this.baseUrl,
        description: 'Organization accessed via PAT'
      };

      return [orgInfo];

    } catch (error) {
      logger.error({
        message: 'Failed to get organizations',
        error: error.message
      });
      throw error;
    }
  }

  /**
   * Get projects
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Projects list
   */
  async getProjects(options = {}) {
    try {
      const response = await this.makeRequest('projects', {
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 1000,
          'stateFilter': options.stateFilter || 'all',
          ...options
        }
      });

      return response.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get projects',
        error: error.message,
        options
      });
      throw error;
    }
  }

  /**
   * Get repositories for a project
   * @param {string} projectId - Project ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Repositories list
   */
  async getRepositories(projectId, options = {}) {
    try {
      const response = await this.makeRequest(`git/repositories`, {
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 1000,
          ...options
        }
      });

      return response.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get repositories',
        error: error.message,
        projectId,
        options
      });
      throw error;
    }
  }

  /**
   * Get pipelines for a project
   * @param {string} projectId - Project ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Pipelines list
   */
  async getPipelines(projectId, options = {}) {
    try {
      const response = await this.makeRequest(`pipelines`, {
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 1000,
          ...options
        }
      });

      return response.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get pipelines',
        error: error.message,
        projectId,
        options
      });
      throw error;
    }
  }

  /**
   * Get work items for a project
   * @param {string} projectId - Project ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Work items list
   */
  async getWorkItems(projectId, options = {}) {
    try {
      // First, create a WIQL query
      const wiqlQuery = {
        query: options.query || 'SELECT [System.Id], [System.Title], [System.State], [System.WorkItemType] FROM WorkItems WHERE [System.TeamProject] = @project ORDER BY [System.ChangedDate] DESC',
        top: options.$top || 50
      };

      const response = await this.makeRequest(`wit/wiql`, {
        method: 'POST',
        params: {
          'api-version': this.apiVersion
        },
        data: wiqlQuery
      });

      if (!response.workItems || response.workItems.length === 0) {
        return [];
      }

      // Get detailed work item information
      const workItemIds = response.workItems.map(wi => wi.id);
      const workItemsResponse = await this.makeRequest(`wit/workitems`, {
        params: {
          'api-version': this.apiVersion,
          ids: workItemIds.join(','),
          '$fields': options.fields || 'System.Id,System.Title,System.Description,System.State,System.WorkItemType,System.AssignedTo,System.CreatedBy,System.CreatedDate,System.ChangedDate,System.Tags'
        }
      });

      return workItemsResponse.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get work items',
        error: error.message,
        projectId,
        options
      });
      throw error;
    }
  }

  /**
   * Get builds for a project
   * @param {string} projectId - Project ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Builds list
   */
  async getBuilds(projectId, options = {}) {
    try {
      const response = await this.makeRequest(`build/builds`, {
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 100,
          'statusFilter': options.status || 'all',
          'resultFilter': options.result || 'all',
          'definitions': options.definitions,
          'branchName': options.branchName,
          ...options
        }
      });

      return response.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get builds',
        error: error.message,
        projectId,
        options
      });
      throw error;
    }
  }

  /**
   * Get releases for a project
   * @param {string} projectId - Project ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Releases list
   */
  async getReleases(projectId, options = {}) {
    try {
      // Note: Release API is on a different domain
      const releaseUrl = this.baseUrl.replace('dev.azure.com', 'vsrm.dev.azure.com');
      const url = `${releaseUrl}/${projectId}/_apis/release/releases`;
      
      const response = await axios(url, {
        headers: this.getAuthHeaders(),
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 100,
          'statusFilter': options.status || 'all',
          ...options
        }
      });

      return response.data.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get releases',
        error: error.message,
        projectId,
        options
      });
      throw error;
    }
  }

  /**
   * Trigger a pipeline run
   * @param {string} projectId - Project ID
   * @param {string} pipelineId - Pipeline ID
   * @param {Object} options - Run options
   * @returns {Promise<Object>} Run information
   */
  async triggerPipelineRun(projectId, pipelineId, options = {}) {
    try {
      const runData = {
        resources: {
          repositories: {
            self: {
              refName: options.branch || 'refs/heads/main'
            }
          }
        },
        variables: options.variables || {},
        ...options
      };

      const response = await this.makeRequest(`pipelines/${pipelineId}/runs`, {
        method: 'POST',
        params: {
          'api-version': this.apiVersion
        },
        data: runData
      });

      return response;

    } catch (error) {
      logger.error({
        message: 'Failed to trigger pipeline run',
        error: error.message,
        projectId,
        pipelineId,
        options
      });
      throw error;
    }
  }

  /**
   * Get pipeline run details
   * @param {string} projectId - Project ID
   * @param {string} pipelineId - Pipeline ID
   * @param {string} runId - Run ID
   * @returns {Promise<Object>} Run details
   */
  async getPipelineRun(projectId, pipelineId, runId) {
    try {
      const response = await this.makeRequest(`pipelines/${pipelineId}/runs/${runId}`, {
        params: {
          'api-version': this.apiVersion
        }
      });

      return response;

    } catch (error) {
      logger.error({
        message: 'Failed to get pipeline run',
        error: error.message,
        projectId,
        pipelineId,
        runId
      });
      throw error;
    }
  }

  /**
   * Get pipeline runs history
   * @param {string} projectId - Project ID
   * @param {string} pipelineId - Pipeline ID
   * @param {Object} options - Query options
   * @returns {Promise<Array>} Runs list
   */
  async getPipelineRuns(projectId, pipelineId, options = {}) {
    try {
      const response = await this.makeRequest(`pipelines/${pipelineId}/runs`, {
        params: {
          'api-version': this.apiVersion,
          '$top': options.$top || 100,
          'statusFilter': options.status || 'all',
          'resultFilter': options.result || 'all',
          ...options
        }
      });

      return response.value || [];

    } catch (error) {
      logger.error({
        message: 'Failed to get pipeline runs',
        error: error.message,
        projectId,
        pipelineId,
        options
      });
      throw error;
    }
  }
}

module.exports = new AzureDevOpsService();
