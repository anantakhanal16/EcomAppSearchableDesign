import { useEffect, useMemo, useState } from 'react';
import { apiRequest, resolveProductImage } from './api';

const initialRegister = { email: '', password: '', fullName: '', role: 'User' };
const initialLogin = { email: '', password: '' };
const initialCheckout = { customerName: '', customerEmail: '' };

export default function App() {
  const [token, setToken] = useState(() => localStorage.getItem('ecom_token') ?? '');
  const [authMode, setAuthMode] = useState('login');
  const [registerForm, setRegisterForm] = useState(initialRegister);
  const [loginForm, setLoginForm] = useState(initialLogin);
  const [checkoutForm, setCheckoutForm] = useState(initialCheckout);

  const [products, setProducts] = useState([]);
  const [cart, setCart] = useState(null);
  const [orders, setOrders] = useState([]);

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const isLoggedIn = Boolean(token);

  useEffect(() => {
    if (!token) {
      setProducts([]);
      setCart(null);
      setOrders([]);
      return;
    }

    void bootstrapData();
  }, [token]);

  async function bootstrapData() {
    try {
      setLoading(true);
      setError('');
      const [productsRes, cartRes, orderRes] = await Promise.all([
        apiRequest('/api/Product/get-all-products?PageNumber=1&PageSize=20', { token }),
        apiRequest('/api/Cart/get-cart', { token }),
        apiRequest('/api/Order/get-user-orders?PageNumber=1&PageSize=10', { token })
      ]);

      setProducts(productsRes.data?.items ?? []);
      setCart(cartRes.data ?? null);
      setOrders(orderRes.data?.items ?? []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  const cartTotal = useMemo(() => {
    if (!cart?.items?.length) return 0;
    return cart.items.reduce((sum, item) => sum + Number(item.unitPrice) * Number(item.quantity), 0);
  }, [cart]);

  function persistToken(nextToken) {
    setToken(nextToken);
    if (nextToken) {
      localStorage.setItem('ecom_token', nextToken);
    } else {
      localStorage.removeItem('ecom_token');
    }
  }

  async function onRegister(e) {
    e.preventDefault();
    try {
      setLoading(true);
      setError('');
      setMessage('');
      const res = await apiRequest('/api/Account/register', { method: 'POST', body: registerForm });
      setMessage(res.message ?? 'Registration successful. Please log in.');
      setRegisterForm(initialRegister);
      setAuthMode('login');
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function onLogin(e) {
    e.preventDefault();
    try {
      setLoading(true);
      setError('');
      setMessage('');
      const res = await apiRequest('/api/Account/login', { method: 'POST', body: loginForm });
      persistToken(res.data?.accessToken ?? '');
      setMessage('Logged in successfully.');
      setLoginForm(initialLogin);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function addToCart(productId) {
    try {
      setLoading(true);
      setError('');
      setMessage('');
      const res = await apiRequest('/api/Cart/add-item', {
        method: 'POST',
        token,
        body: { productID: productId, quantity: 1 }
      });
      setCart(res.data);
      setMessage('Item added to cart.');
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function createOrderFromCart() {
    if (!cart?.items?.length) {
      setError('Cart is empty.');
      return;
    }

    if (!checkoutForm.customerName.trim() || !checkoutForm.customerEmail.trim()) {
      setError('Customer name and email are required to checkout.');
      return;
    }

    const nowIso = new Date().toISOString();
    const payload = {
      orderDate: nowIso,
      customerName: checkoutForm.customerName.trim(),
      customerEmail: checkoutForm.customerEmail.trim(),
      totalAmount: Number(cartTotal.toFixed(2)),
      orderDetails: cart.items.map((item) => ({
        productID: item.productID,
        productPrice: Math.round(Number(item.unitPrice)),
        quantity: item.quantity,
        subTotal: Number(item.unitPrice) * Number(item.quantity)
      }))
    };

    try {
      setLoading(true);
      setError('');
      setMessage('');
      const orderResponse = await apiRequest('/api/Order/create-order', {
        method: 'POST',
        token,
        body: payload
      });

      await apiRequest('/api/Cart/clear-cart', { method: 'DELETE', token });
      setCheckoutForm(initialCheckout);
      await bootstrapData();
      setMessage(`Order #${orderResponse.data?.orderID ?? ''} created successfully.`);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  function logout() {
    persistToken('');
    setMessage('Logged out.');
    setError('');
  }

  return (
    <div className="page">
      <header>
        <h1>Ecom React Storefront</h1>
        <p>UI client for the ASP.NET e-commerce API.</p>
      </header>

      {message && <p className="alert success">{message}</p>}
      {error && <p className="alert error">{error}</p>}

      {!isLoggedIn ? (
        <section className="card auth">
          <div className="tabs">
            <button className={authMode === 'login' ? 'active' : ''} onClick={() => setAuthMode('login')}>
              Login
            </button>
            <button className={authMode === 'register' ? 'active' : ''} onClick={() => setAuthMode('register')}>
              Register
            </button>
          </div>

          {authMode === 'login' ? (
            <form onSubmit={onLogin}>
              <input placeholder="Email" type="email" value={loginForm.email} onChange={(e) => setLoginForm((v) => ({ ...v, email: e.target.value }))} required />
              <input placeholder="Password" type="password" value={loginForm.password} onChange={(e) => setLoginForm((v) => ({ ...v, password: e.target.value }))} required />
              <button disabled={loading}>{loading ? 'Please wait...' : 'Login'}</button>
            </form>
          ) : (
            <form onSubmit={onRegister}>
              <input placeholder="Full Name" value={registerForm.fullName} onChange={(e) => setRegisterForm((v) => ({ ...v, fullName: e.target.value }))} required />
              <input placeholder="Email" type="email" value={registerForm.email} onChange={(e) => setRegisterForm((v) => ({ ...v, email: e.target.value }))} required />
              <input placeholder="Password" type="password" value={registerForm.password} onChange={(e) => setRegisterForm((v) => ({ ...v, password: e.target.value }))} required />
              <select value={registerForm.role} onChange={(e) => setRegisterForm((v) => ({ ...v, role: e.target.value }))}>
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
              <button disabled={loading}>{loading ? 'Please wait...' : 'Create Account'}</button>
            </form>
          )}
        </section>
      ) : (
        <>
          <div className="toolbar">
            <button onClick={() => void bootstrapData()} disabled={loading}>Refresh Data</button>
            <button onClick={logout}>Logout</button>
          </div>

          <main className="layout">
            <section className="card">
              <h2>Products</h2>
              <div className="products">
                {products.map((product) => (
                  <article key={product.productID} className="product">
                    {product.productImage && (
                      <img src={resolveProductImage(product.productImage)} alt={product.productName} />
                    )}
                    <h3>{product.productName}</h3>
                    <p>{product.category}</p>
                    <p>${Number(product.price).toFixed(2)}</p>
                    <button onClick={() => void addToCart(product.productID)} disabled={loading || !product.isActive}>
                      {product.isActive ? 'Add to Cart' : 'Unavailable'}
                    </button>
                  </article>
                ))}
              </div>
            </section>

            <section className="card">
              <h2>Cart</h2>
              {!cart?.items?.length ? (
                <p>No items in cart.</p>
              ) : (
                <>
                  <ul>
                    {cart.items.map((item) => (
                      <li key={item.cartItemID}>
                        {item.productName} × {item.quantity} — ${(Number(item.unitPrice) * Number(item.quantity)).toFixed(2)}
                      </li>
                    ))}
                  </ul>
                  <p className="total">Total: ${cartTotal.toFixed(2)}</p>
                  <input
                    placeholder="Customer name"
                    value={checkoutForm.customerName}
                    onChange={(e) => setCheckoutForm((v) => ({ ...v, customerName: e.target.value }))}
                  />
                  <input
                    placeholder="Customer email"
                    type="email"
                    value={checkoutForm.customerEmail}
                    onChange={(e) => setCheckoutForm((v) => ({ ...v, customerEmail: e.target.value }))}
                  />
                  <button onClick={() => void createOrderFromCart()} disabled={loading}>Checkout</button>
                </>
              )}
            </section>

            <section className="card">
              <h2>Recent Orders</h2>
              {!orders.length ? (
                <p>No orders yet.</p>
              ) : (
                <ul>
                  {orders.map((order) => (
                    <li key={order.orderID}>
                      <strong>#{order.orderID}</strong> — {order.orderStatus} — ${Number(order.totalAmount).toFixed(2)}
                    </li>
                  ))}
                </ul>
              )}
            </section>
          </main>
        </>
      )}
    </div>
  );
}
