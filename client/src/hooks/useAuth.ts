import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { RootState, AppDispatch } from '../store';
import { getCurrentUser, login, logout, clearError } from '../store/slices/authSlice';

export const useAuth = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { user, token, isAuthenticated, isLoading, error } = useSelector(
    (state: RootState) => state.auth
  );

  useEffect(() => {
    // If we have a token but no user, try to get the current user
    if (token && !user && !isLoading) {
      dispatch(getCurrentUser());
    }
  }, [dispatch, token, user, isLoading]);

  const handleLogin = async (credentials: { username: string; password: string }) => {
    return dispatch(login(credentials));
  };

  const handleLogout = async () => {
    return dispatch(logout());
  };

  const handleClearError = () => {
    dispatch(clearError());
  };

  return {
    user,
    token,
    isAuthenticated,
    isLoading,
    error,
    login: handleLogin,
    logout: handleLogout,
    clearError: handleClearError,
  };
};
