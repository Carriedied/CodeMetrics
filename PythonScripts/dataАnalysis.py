import tensorflow as tf
import pandas as pd
import psycopg2
from ollama import chat
from ollama import ChatResponse


# Параметры подключения к БД
DB_CONFIG = {
    'dbname': 'postgres',
    'user': 'postgres',
    'password': 'postgres',
    'host': 'localhost',
    'port': '5432',
}

def connect_db():
    """Подключение к базе данных"""
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        return conn
    except Exception as e:
        print(f"Ошибка подключения к БД: {e}")
        return None



def load_data_from_db():
    """Выгрузка данных из всех таблиц"""
    conn = connect_db()
    if not conn:
        return None

    data = {}

    try:
        # Список таблиц для анализа
        tables = [
            'users', 'projects', 'repositories', 'repo_branches',
            'git_users', 'repo_commits', 'repo_commit_parents',
            'repo_files', 'issues', 'pull_requests',
            'pull_request_reviewers', 'webhooks', 'service_accounts'
        ]


        for table in tables:
            query = f"SELECT * FROM {table}"
            data[table] = pd.read_sql_query(query, conn)

        conn.close()
        return data

    except Exception as e:
        print(f"Ошибка загрузки данных: {e}")
        conn.close()
        return None


def calculate_basic_statistics(data):
    """Расчет базовой статистики с использованием TensorFlow"""
    stats = {}

    # Статистика по пользователям
    users_df = data['users']
    stats['total_users'] = tf.constant(len(users_df), dtype=tf.float32)
    stats['active_users'] = tf.constant(users_df['is_active'].sum(), dtype=tf.float32)
    stats['active_users_ratio'] = stats['active_users'] / stats['total_users']

    # Статистика по проектам
    projects_df = data['projects']
    stats['total_projects'] = tf.constant(len(projects_df), dtype=tf.float32)
    stats['public_projects'] = tf.constant(projects_df['is_public'].sum(), dtype=tf.float32)
    stats['favorite_projects'] = tf.constant(projects_df['is_favorite'].sum(), dtype=tf.float32)

    # Статистика по репозиториям
    repos_df = data['repositories']
    stats['total_repositories'] = tf.constant(len(repos_df), dtype=tf.float32)

    # Статистика по коммитам
    commits_df = data['repo_commits']
    stats['total_commits'] = tf.constant(len(commits_df), dtype=tf.float32)

    # Статистика по веткам
    branches_df = data['repo_branches']
    stats['total_branches'] = tf.constant(len(branches_df), dtype=tf.float32)
    stats['protected_branches'] = tf.constant(branches_df['is_protected'].sum(), dtype=tf.float32)

    # Статистика по pull requests
    pr_df = data['pull_requests']
    stats['total_pr'] = tf.constant(len(pr_df), dtype=tf.float32)
    stats['merged_pr'] = tf.constant(pr_df['status'].str.lower().str.contains('merged').sum(), dtype=tf.float32)

    # Статистика по issues
    issues_df = data['issues']
    stats['total_issues'] = tf.constant(len(issues_df), dtype=tf.float32)

    return stats


def calculate_advanced_metrics(data):
    """Расчет расширенных метрик с использованием TensorFlow"""
    metrics = {}

    # Анализ активности коммитов
    commits_df = data['repo_commits']
    if not commits_df.empty:
        commits_df['created_at'] = pd.to_datetime(commits_df['created_at'])
        commit_dates = commits_df['created_at'].dt.date

        # Количество коммитов по дням (используем TensorFlow)
        unique_days = commit_dates.nunique()
        total_commits = len(commits_df)

        metrics['commits_per_day_avg'] = tf.constant(total_commits / unique_days if unique_days > 0 else 0,
                                                     dtype=tf.float32)

        # Распределение коммитов по авторам
        author_commits = commits_df['author_id'].value_counts()
        metrics['commits_per_author_avg'] = tf.constant(author_commits.mean(), dtype=tf.float32)
        metrics['commits_per_author_std'] = tf.constant(author_commits.std(), dtype=tf.float32)

    # Анализ pull requests
    pr_df = data['pull_requests']
    if not pr_df.empty:
        pr_df['created_at'] = pd.to_datetime(pr_df['created_at'])
        pr_df['updated_at'] = pd.to_datetime(pr_df['updated_at'])

        # Время до мержа PR
        merged_pr = pr_df[pr_df['status'].str.lower() == 'merged']
        if not merged_pr.empty:
            merge_times = (merged_pr['merged_at'] - merged_pr['created_at']).dt.total_seconds() / 3600  # в часах
            metrics['pr_merge_time_avg_hours'] = tf.constant(merge_times.mean(), dtype=tf.float32)

    return metrics


def create_tensorflow_model_for_analysis():
    """Создание TensorFlow модели для анализа метрик"""
    model = tf.keras.Sequential([
        tf.keras.layers.Dense(64, activation='relu', input_shape=(10,)),
        tf.keras.layers.Dropout(0.2),
        tf.keras.layers.Dense(32, activation='relu'),
        tf.keras.layers.Dense(16, activation='relu'),
        tf.keras.layers.Dense(1, activation='linear')
    ])

    model.compile(optimizer='adam',
                  loss='mse',
                  metrics=['mae'])

    return model


def main():
    """Основная функция"""

    data = load_data_from_db()

    if data is None:

        return


    stats = calculate_basic_statistics(data)
    metrics = calculate_advanced_metrics(data)


    model = create_tensorflow_model_for_analysis()


    # Формирование content для передачи в Ollama
    content = "Проанализируй общую информацию GIT METRICS и дай кратко свои рекомендации, ответ не должен быть больше 20 предложений\n\n"

    content += "📊 ПОЛЬЗОВАТЕЛИ:\n"
    content += f"   Всего пользователей: {stats['total_users'].numpy():.0f}\n"
    content += f"   Активных пользователей: {stats['active_users'].numpy():.0f}\n"
    content += f"   Доля активных: {stats['active_users_ratio'].numpy():.1%}\n\n"

    content += "🏗️  ПРОЕКТЫ:\n"
    content += f"   Всего проектов: {stats['total_projects'].numpy():.0f}\n"
    content += f"   Публичных проектов: {stats['public_projects'].numpy():.0f}\n"
    content += f"   Избранных проектов: {stats['favorite_projects'].numpy():.0f}\n\n"

    content += "📁 РЕПОЗИТОРИИ:\n"
    content += f"   Всего репозиториев: {stats['total_repositories'].numpy():.0f}\n"
    content += f"   Всего веток: {stats['total_branches'].numpy():.0f}\n"
    content += f"   Защищенных веток: {stats['protected_branches'].numpy():.0f}\n\n"

    content += "🔨 АКТИВНОСТЬ:\n"
    content += f"   Всего коммитов: {stats['total_commits'].numpy():.0f}\n"
    content += f"   Всего Pull Request'ов: {stats['total_pr'].numpy():.0f}\n"
    content += f"   Мерженых PR: {stats['merged_pr'].numpy():.0f}\n"
    content += f"   Всего Issues: {stats['total_issues'].numpy():.0f}\n\n"

    content += "📈 РАСШИРЕННЫЕ МЕТРИКИ:\n"
    if 'commits_per_day_avg' in metrics:
        content += f"   Среднее коммитов в день: {metrics['commits_per_day_avg'].numpy():.2f}\n"
    if 'commits_per_author_avg' in metrics:
        content += f"   Среднее коммитов на автора: {metrics['commits_per_author_avg'].numpy():.2f}\n"
    if 'pr_merge_time_avg_hours' in metrics:
        content += f"   Среднее время мержа PR (часы): {metrics['pr_merge_time_avg_hours'].numpy():.2f}\n\n"

    content += "🧮 ДОПОЛНИТЕЛЬНЫЙ АНАЛИЗ:\n"

    # Расчет отношения коммитов на пользователя
    commit_tensor = tf.constant([stats['total_commits'].numpy()], dtype=tf.float32)
    user_tensor = tf.constant([stats['total_users'].numpy()], dtype=tf.float32)
    commits_per_user = commit_tensor / user_tensor
    content += f"   Коммитов на пользователя: {commits_per_user.numpy()[0]:.2f}\n"

    # Расчет эффективности PR
    if stats['total_pr'].numpy() > 0:
        pr_efficiency = stats['merged_pr'] / stats['total_pr']
        content += f"   Эффективность PR: {pr_efficiency.numpy():.1%}\n"

    # Добавляем информацию о коммитах
    content += "\n📋 КОММИТЫ С СООБЩЕНИЯМИ:\n"
    commits_df = data['repo_commits'].copy()
    git_users_df = data['git_users'].copy()
    repos_df = data['repositories'].copy()

    # Объединение данных о коммитах
    commits_with_authors = pd.merge(
        commits_df,
        git_users_df[['id', 'name']],
        left_on='author_id',
        right_on='id',
        how='left'
    )

    commits_full = pd.merge(
        commits_with_authors,
        repos_df[['id', 'name']],
        left_on='repository_id',
        right_on='id',
        how='left',
        suffixes=('_author', '_repo')
    )

    # Сортируем по дате
    commits_sorted = commits_full.sort_values('created_at', ascending=False)

    # Ограничиваем количество выводимых коммитов для удобства
    for idx, commit in commits_sorted.head(10).iterrows():  # первые 10 коммитов
        author_name = commit.get('name_author', 'Unknown')
        repo_name = commit.get('name_repo', 'Unknown')
        commit_hash = commit['hash'][:8] if pd.notna(commit['hash']) else 'Unknown'
        message = commit['message'] if pd.notna(commit['message']) else 'No message'

        commit_date = commit['created_at']
        if hasattr(commit_date, 'strftime'):
            commit_date = commit_date.strftime('%Y-%m-%d %H:%M')
        else:
            commit_date = str(commit_date)[:16]

        content += f"\n👤 {author_name} | 📂 {repo_name} | 🕒 {commit_date} | #{commit_hash}\n"
        content += f"   💬 {message}\n"

    # Используем сформированный content для запроса к Ollama
    response: ChatResponse = chat(model='gemma3', messages=[
        {
            'role': 'user',
            'content': content,
        },
    ])
    return (response['message']['content'])

if __name__ == "__main__":
    main()

